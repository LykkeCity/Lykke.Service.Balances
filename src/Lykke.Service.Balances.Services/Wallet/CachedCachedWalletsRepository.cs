using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Service.Balances.Core.Domain.Wallets;
using Lykke.Service.Balances.Core.Services.Wallets;
using Lykke.Service.Balances.Services.Wallet.CacheModels;
using Microsoft.Extensions.Caching.Distributed;

namespace Lykke.Service.Balances.Services.Wallet
{
    [UsedImplicitly]
    public class CachedCachedWalletsRepository : ICachedWalletsRepository
    {
        private readonly IDistributedCache _cache;
        private readonly IWalletsRepository _repository;
        private readonly TimeSpan _cacheExpiration;
        private readonly ILog _log;

        public CachedCachedWalletsRepository(
            [NotNull] IDistributedCache cache,
            [NotNull] IWalletsRepository repository, 
            TimeSpan cacheExpiration,
            [NotNull] ILog log)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _cacheExpiration = cacheExpiration;
            _log = log.CreateComponentScope(nameof(CachedCachedWalletsRepository));
        }

        public async Task<IReadOnlyList<IWallet>> GetAllAsync(string walletId)
        {
            return await _cache.TryGetFromCacheAsync(
                GetAllBalancesCacheKey(walletId),
                async () => (await _repository.GetAsync(walletId))
                    .Select(CachedWalletModel.Copy)
                    .ToArray(),
                slidingExpiration: _cacheExpiration,
                log: _log);
        }

        public async Task<IWallet> GetAsync(string walletId, string assetId)
        {
            return await _cache.TryGetFromCacheAsync(
                GetAssetBalanceCacheKey(walletId, assetId),
                async () => CachedWalletModel.Copy(await _repository.GetAsync(walletId, assetId)),
                slidingExpiration: _cacheExpiration,
                log: _log);
        }

        public async Task UpdateBalanceAsync(string walletId, string assetId, decimal balance, decimal reserved, long updateSequenceNumber)
        {
            var wallet = CachedWalletModel.Create(assetId, balance, reserved, updateSequenceNumber);

            var updated = await _repository.UpdateBalanceAsync(walletId, wallet);
            if (updated)
            {
                var key = GetAssetBalanceCacheKey(walletId, assetId);
                await _cache.TryUpdateCacheAsync(key, wallet, slidingExpiration: _cacheExpiration, log: _log);
            }
        }

        public Task CacheItAsync(string walletId)
        {
            return GetAllAsync(walletId);
        }

        public async Task<IReadOnlyList<IWallet>> GetTotalBalancesAsync()
        {
            return await _cache.TryGetFromCacheAsync(
                GetTotalBalancesCacheKey(),
                async () => (await _repository.GetTotalBalancesAsync())
                    .Select(CachedWalletModel.Copy)
                    .ToArray(),
                slidingExpiration: _cacheExpiration,
                log: _log);
        }


        private static string GetAllBalancesCacheKey(string clientId) => $":balances:{clientId}:all";
        private static string GetAssetBalanceCacheKey(string clientId, string assetId) => $":balances:{clientId}:{assetId}";

        private static string GetTotalBalancesCacheKey() => ":totalBalances";
    }
}
