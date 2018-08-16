using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.Balances.Core.Domain.Wallets;
using Lykke.Service.Balances.Core.Services.Wallets;
using Lykke.Service.Balances.Services.Wallet.CacheModels;
using Microsoft.Extensions.Caching.Distributed;

namespace Lykke.Service.Balances.Services.Wallet
{
    [UsedImplicitly]
    public class CachedWalletsRepository : ICachedWalletsRepository
    {
        private readonly IDistributedCache _cache;
        private readonly IWalletsRepository _repository;
        private readonly TimeSpan _cacheExpiration;
        private readonly ILog _log;

        public CachedWalletsRepository(
            [NotNull] IDistributedCache cache,
            [NotNull] IWalletsRepository repository, 
            TimeSpan cacheExpiration,
            [NotNull] ILogFactory logFactory)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _cacheExpiration = cacheExpiration;
            _log = logFactory.CreateLog(this);
        }

        public async Task<IReadOnlyList<IWallet>> GetAllAsync(string walletId)
        {
            return await _cache.TryGetAsync(
                GetAllBalancesCacheKey(walletId),
                async () => (await _repository.GetAsync(walletId))
                    .Select(CachedWalletModel.Copy)
                    .ToArray(),
                slidingExpiration: _cacheExpiration,
                log: _log);
        }

        public async Task<IWallet> GetAsync(string walletId, string assetId)
        {
            return await _cache.TryGetAsync(
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
                await _cache.TrySetAsync(key, wallet, slidingExpiration: _cacheExpiration, log: _log);
                await _cache.TryRemoveAsync(GetAllBalancesCacheKey(walletId));
            }
        }

        public Task CacheItAsync(string walletId)
        {
            return GetAllAsync(walletId);
        }

        public async Task<IReadOnlyList<IWallet>> GetTotalBalancesAsync()
        {
            return await _cache.TryGetAsync(
                GetTotalBalancesCacheKey(),
                async () => (await _repository.GetTotalBalancesAsync())
                    .Select(CachedWalletModel.Copy)
                    .ToArray(),
                slidingExpiration: _cacheExpiration,
                log: _log);
        }


        private static string GetAllBalancesCacheKey(string walletId) => $":balances:{walletId}:all";
        private static string GetAssetBalanceCacheKey(string walletId, string assetId) => $":balances:{walletId}:{assetId}";

        private static string GetTotalBalancesCacheKey() => ":totalBalances";
    }
}
