using Lykke.Service.Balances.Core.Domain;
using Lykke.Service.Balances.Core.Services;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.Services
{
    public class TotalBalancesService : ITotalBalancesService
    {
        private readonly IDistributedCache _cache;
        private static readonly DistributedCacheEntryOptions CacheOptions = new DistributedCacheEntryOptions();

        public TotalBalancesService(IDistributedCache cache)
        {
            _cache = cache;
        }

        private static string GetTotalBalancesCacheKey() => ":totalBalances";
        public Task<IReadOnlyList<TotalAssetBalance>> GetTotalBalancesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<TotalAssetBalance> GetTotalAssetBalanceAsync(string assetId)
        {
            throw new NotImplementedException();
        }

        public Task ChangeTotalBalanceAsync(string assetId, decimal delta, long sequenceNumber)
        {
            // todo: save delta to Redis
            throw new NotImplementedException();
            //return _cache.SetAsync(key, value, CacheOptions);
        }
    }
}
