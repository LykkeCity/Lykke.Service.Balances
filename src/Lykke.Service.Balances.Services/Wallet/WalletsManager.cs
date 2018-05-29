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
    public class WalletsManager : IWalletsManager
    {
        private readonly IDistributedCache _cache;
        private readonly IWalletsRepository _repository;
        private readonly TimeSpan _cacheExpiration;

        public WalletsManager(IDistributedCache cache, IWalletsRepository repository, TimeSpan cacheExpiration)
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

        /// <remarks>
        /// Method calls with single <paramref name="walletId"/>, should be synchornized
        /// </remarks>
        public async Task UpdateBalanceAsync(string walletId, IEnumerable<(string Asset, decimal Balance, decimal Reserved)> assetBalances)
        {
            // NOTE: This is not atomic cache update. Due to this, service can't be scaled out.
            var wallets = new List<IWallet>();
            var tasks = new List<Task>();
            
            foreach (var assetBalance in assetBalances)
            {
                wallets.Add(new Core.Domain.Wallets.Wallet{AssetId = assetBalance.Asset, Balance = assetBalance.Balance, Reserved = assetBalance.Reserved});
                string key = GetAssetBalanceCacheKey(walletId, assetBalance.Asset);
                
                var cachedWallet = CachedWalletModel.Create(assetBalance.Asset, assetBalance.Balance, assetBalance.Reserved);
                
                tasks.Add(_cache.UpdateCacheAsync(key, cachedWallet, slidingExpiration: _cacheExpiration));
            }

            if (wallets.Any())
                tasks.Add(_repository.UpdateBalanceAsync(walletId, wallets));
            
            tasks.Add(_cache.RemoveAsync(GetAllBalancesCacheKey(walletId)));

            await Task.WhenAll(tasks);
        }

        public async Task CacheItAsync(string walletId)
        {
            var storedValue = await GetAllAsync(walletId);

            await _cache.UpdateCacheAsync(GetAllBalancesCacheKey(walletId), storedValue, slidingExpiration: _cacheExpiration);
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

        public async Task UpdateTotalBalancesAsync(List<Core.Domain.Wallets.Wallet> totalBalances)
        {
            var tasks = new List<Task>();

            foreach (var balance in totalBalances)
            {
                string key = GetTotalAssetBalanceCacheKey(balance.AssetId);
                
                var currentBalance = CachedWalletModel.Create(balance.AssetId, balance.Balance, balance.Reserved);
                
                tasks.Add(_cache.UpdateCacheAsync(key, currentBalance, slidingExpiration: _cacheExpiration));
            }
            
            if (totalBalances.Any())
                tasks.Add(_cache.RemoveAsync(GetTotalBalancesCacheKey()));
            
            await Task.WhenAll(tasks);
        }

        private static string GetAllBalancesCacheKey(string clientId) => $":balances:{clientId}:all";
        private static string GetAssetBalanceCacheKey(string clientId, string assetId) => $":balances:{clientId}:{assetId}";

        private static string GetTotalBalancesCacheKey() => ":totalBalances";
        private static string GetTotalAssetBalanceCacheKey(string assetId) => $":totalBalance:{assetId}";
    }
}
