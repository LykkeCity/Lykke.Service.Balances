using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public CachedCachedWalletsRepository(IDistributedCache cache, IWalletsRepository repository, TimeSpan cacheExpiration)
        {
            _cache = cache;
            _repository = repository;
            _cacheExpiration = cacheExpiration;
        }

        public async Task<IReadOnlyList<IWallet>> GetAllAsync(string walletId)
        {
            return await _cache.TryGetFromCacheAsync(
                GetAllBalancesCacheKey(walletId),
                async () => (await _repository.GetAsync(walletId))
                    .Select(CachedWalletModel.Copy)
                    .ToArray(),
                slidingExpiration: _cacheExpiration);
        }

        public async Task<IWallet> GetAsync(string walletId, string assetId)
        {
            return await _cache.TryGetFromCacheAsync(
                GetAssetBalanceCacheKey(walletId, assetId),
                async () => CachedWalletModel.Copy(await _repository.GetAsync(walletId, assetId)),
                slidingExpiration: _cacheExpiration);
        }

        public async Task UpdateBalanceAsync(string walletId, string assetId, decimal balance, decimal reserved, long updateSequenceNumber)
        {
            var wallet = CachedWalletModel.Create(assetId, balance, reserved, updateSequenceNumber);

            var updated = await _repository.UpdateBalanceAsync(walletId, wallet);
            if (updated)
            {
                try
                {
                    var key = GetAssetBalanceCacheKey(walletId, assetId);
                    await _cache.UpdateCacheAsync(key, wallet, slidingExpiration: _cacheExpiration);
                }
                catch (Exception e)
                {
                    // ignoring the errors
                    // todo: implement fire-and-forget code by calling Redis library directly
                }
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
                slidingExpiration: _cacheExpiration);
        }


        private static string GetAllBalancesCacheKey(string clientId) => $":balances:{clientId}:all";
        private static string GetAssetBalanceCacheKey(string clientId, string assetId) => $":balances:{clientId}:{assetId}";

        private static string GetTotalBalancesCacheKey() => ":totalBalances";
    }
}
