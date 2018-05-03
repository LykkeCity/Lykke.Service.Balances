using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

        public async Task<IReadOnlyList<IWallet>> GetAsync(string walletId)
        {
            return await _cache.TryGetFromCacheAsync(
                GetCacheKey(walletId),
                async () => (await _repository.GetAsync(walletId))
                    .Select(CachedWalletModel.Copy)
                    .ToArray(),
                slidingExpiration: _cacheExpiration);
        }

        public async Task<IWallet> GetAsync(string walletId, string assetId)
        {
            return (await GetAsync(walletId)).FirstOrDefault(itm => itm.AssetId == assetId);
        }

        /// <remarks>
        /// Method calls with single <paramref name="walletId"/>, should be synchornized
        /// </remarks>
        public async Task UpdateBalanceAsync(string walletId, IEnumerable<(string Asset, decimal Balance, decimal Reserved)> assetBalances)
        {
            // NOTE: This is not atomic cache update. Due to this, service can't be scaled out.

            var cacheKey = GetCacheKey(walletId);
            var cachedValue = (await GetAsync(walletId)).Select(CachedWalletModel.Copy).ToList();
            var wallets = new List<IWallet>();

            foreach (var assetBalance in assetBalances)
            {
                wallets.Add(new Core.Domain.Wallets.Wallet{AssetId = assetBalance.Asset, Balance = assetBalance.Balance, Reserved = assetBalance.Reserved});
                var cachedWallet = cachedValue.FirstOrDefault(w => w.AssetId == assetBalance.Asset);

                if (cachedWallet != null)
                {
                    cachedWallet.Update(assetBalance.Balance, assetBalance.Reserved);
                }
                else
                {
                    var newWallet = CachedWalletModel.Create(assetBalance.Asset, assetBalance.Balance, assetBalance.Reserved);

                    cachedValue.Add(newWallet);
                }
            }
            
            if (wallets.Any())
                await _repository.UpdateBalanceAsync(walletId, wallets);

            await _cache.UpdateCacheAsync(cacheKey, cachedValue, slidingExpiration: _cacheExpiration);
        }

        public async Task CacheItAsync(string walletId)
        {
            var storedValue = (await _repository.GetAsync(walletId))
                .Select(CachedWalletModel.Copy)
                .ToArray();

            await _cache.UpdateCacheAsync(GetCacheKey(walletId), storedValue, slidingExpiration: _cacheExpiration);
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
            var balances = (await GetTotalBalancesAsync()).Select(CachedWalletModel.Copy).ToList();

            foreach (var balance in totalBalances)
            {
                var currentBalance = balances.FirstOrDefault(item => item.AssetId == balance.AssetId);

                if (currentBalance != null)
                {
                    currentBalance.Update(currentBalance.Balance + balance.Balance, currentBalance.Reserved + balance.Reserved);
                }
                else
                {
                    balances.Add(CachedWalletModel.Create(balance.AssetId, balance.Balance, balance.Reserved));
                }
            }
            
            await _cache.UpdateCacheAsync(GetTotalBalancesCacheKey(), balances, slidingExpiration: _cacheExpiration);
            await _repository.UpdateTotalBalancesAsync(balances.Select(CachedWalletModel.Copy));
        }

        private static string GetCacheKey(string clientId) => $":balances:{clientId}";

        private static string GetTotalBalancesCacheKey() => ":totalBalances";
    }
}
