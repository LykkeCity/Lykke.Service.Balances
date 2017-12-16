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

        public async Task<IReadOnlyList<IWallet>> GetAsync(string clientId)
        {
            return await _cache.TryGetFromCacheAsync(
                GetCacheKey(clientId),
                async () => (await _repository.GetAsync(clientId))
                    .Select(CachedWalletModel.Copy)
                    .ToArray(),
                slidingExpiration: _cacheExpiration);
        }

        public async Task<IWallet> GetAsync(string clientId, string assetId)
        {
            return (await GetAsync(clientId)).FirstOrDefault(itm => itm.AssetId == assetId);
        }

        /// <remarks>
        /// Method calls with single <paramref name="clientId"/>, should be synchornized
        /// </remarks>
        public async Task UpdateBalanceAsync(string clientId, IEnumerable<(string Asset, double Balance, double Reserved)> assetBalances)
        {
            // NOTE: This is not atomic cache update. Due to this, service can't be scaled out.

            var cacheKey = GetCacheKey(clientId);
            var cachedValue = await _cache.TryGetFromCacheAsync<List<CachedWalletModel>>(cacheKey);

            if (cachedValue != null)
            {
                foreach (var assetBalance in assetBalances)
                {
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

                await _cache.UpdateCacheAsync(cacheKey, cachedValue, slidingExpiration: _cacheExpiration);
            }

            // TODO: Update storage, when ME will be ready to stop direct writing to the storage
        }

        public async Task CacheItAsync(string clientId)
        {
            var storedValue = (await _repository.GetAsync(clientId))
                .Select(CachedWalletModel.Copy)
                .ToArray();

            await _cache.UpdateCacheAsync(GetCacheKey(clientId), storedValue, slidingExpiration: _cacheExpiration);
        }

        private static string GetCacheKey(string clientId)
        {
            return $":{clientId}:Total";
        }
    }
}