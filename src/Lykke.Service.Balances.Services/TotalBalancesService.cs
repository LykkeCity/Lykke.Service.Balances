using Lykke.Service.Balances.Core.Domain;
using Lykke.Service.Balances.Core.Services;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.Services
{
    public class TotalBalancesService : ITotalBalancesService
    {
        private static readonly string PartitionKey = "SpotClientBalances:total";
        private readonly IDatabase _redisDatabase;

        public TotalBalancesService(IDatabase redisDatabase)
        {
            _redisDatabase = redisDatabase;
        }

        public async Task<IReadOnlyList<TotalAssetBalance>> GetTotalBalancesAsync()
        {
            var balances = await _redisDatabase.HashGetAllAsync(PartitionKey);
            return balances.Select(x => new TotalAssetBalance
            {
                AssetId = x.Name,
                Balance = (decimal)(double)x.Value
            }).ToList();
        }

        public async Task<TotalAssetBalance> GetTotalAssetBalanceAsync(string assetId)
        {
            var balance = await _redisDatabase.HashGetAsync(PartitionKey, assetId);
            return new TotalAssetBalance
            {
                AssetId = assetId,
                Balance = (decimal)(double)balance
            };
        }

        public async Task ChangeTotalBalanceAsync(string assetId, decimal delta, long sequenceNumber)
        {
            await _redisDatabase.HashIncrementAsync(PartitionKey, assetId, (double)delta);
        }
    }
}
