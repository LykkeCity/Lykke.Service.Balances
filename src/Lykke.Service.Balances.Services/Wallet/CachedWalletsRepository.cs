using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.Balances.Core.Domain;
using Lykke.Service.Balances.Core.Services.Wallets;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.Services.Wallet
{
    [UsedImplicitly]
    public class CachedWalletsRepository : ICachedWalletsRepository
    {
        private readonly IDatabase _redisDatabase;
        private readonly string _partitionKey;
        private readonly IWalletsRepository _repository;
        private readonly TimeSpan _cacheExpiration;
        private readonly ILog _log;

        public CachedWalletsRepository(
            [NotNull] IDistributedCache cache,
            [NotNull] IWalletsRepository repository,
            TimeSpan cacheExpiration,
            [NotNull] ILogFactory logFactory,
            [NotNull] IDatabase redisDatabase,
            [NotNull] string partitionKey)
        {
            if (logFactory == null) throw new ArgumentNullException(nameof(logFactory));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _cacheExpiration = cacheExpiration;
            _redisDatabase = redisDatabase ?? throw new ArgumentNullException(nameof(redisDatabase));
            _partitionKey = partitionKey ?? throw new ArgumentNullException(nameof(partitionKey));
            _log = logFactory.CreateLog(this);
        }

        public async Task<IReadOnlyList<IWallet>> GetAllAsync(string walletId)
        {
            // todo: refactor most of code below into RedisCacheExtensions method 'TryHashGetAllAsync'
            try
            {
                var balances = await _redisDatabase.HashGetAllAsync(GetCacheKey(walletId));
                if (balances != null && balances.Length > 0)
                {
                    return balances.Select(x => CacheSerializer.Deserialize<CachedWalletModel>(x.Value)).ToList();
                }
            }
            catch (RedisConnectionException ex)
            {
                _log.Warning("Redis cache is not available", ex);
            }

            return await ReloadAllAsync(walletId);
        }

        private async Task<IReadOnlyList<IWallet>> ReloadAllAsync(string walletId)
        {
            var result = (await _repository.GetAsync(walletId)).Select(CachedWalletModel.Create).ToArray();

            try
            {
                await _redisDatabase.HashSetAsync(
                    GetCacheKey(walletId),
                    result.Select(x => new HashEntry(x.AssetId, CacheSerializer.Serialize(x))).ToArray());
                await _redisDatabase.KeyExpireAsync(GetCacheKey(walletId), _cacheExpiration);
            }
            catch (RedisConnectionException ex)
            {
                _log.Warning("Redis cache is not available", ex);
            }

            return result;
        }

        public async Task<IWallet> GetAsync(string walletId, string assetId)
        {
            return await _redisDatabase.TryHashGetAsync(
                GetCacheKey(walletId),
                assetId,
                async () => CachedWalletModel.Create(await _repository.GetAsync(walletId, assetId)),
                async () => await ReloadAllAsync(walletId),
                _cacheExpiration,
                _log);
        }

        public async Task UpdateBalanceAsync(string walletId, string assetId, decimal balance, decimal reserved, long updateSequenceNumber)
        {
            var wallet = CachedWalletModel.Create(assetId, balance, reserved, updateSequenceNumber);

            var updated = await _repository.UpdateBalanceAsync(walletId, wallet, updateSequenceNumber);
            if (updated)
            {
                var cacheKey = GetCacheKey(walletId);
                try
                {
                    if (await _redisDatabase.KeyExistsAsync(cacheKey))
                    {
                        await _redisDatabase.TryHashSetAsync(
                            cacheKey,
                            assetId,
                            wallet,
                            _cacheExpiration,
                            _log);
                    }
                }
                catch (RedisConnectionException ex)
                {
                    _log.Warning("Redis cache is not available", ex);
                }
            }
        }

        public Task CacheItAsync(string walletId)
        {
            return ReloadAllAsync(walletId);
        }

        private string GetCacheKey(string walletId) => $"{_partitionKey}:{walletId}";
    }
}
