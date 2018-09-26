using JetBrains.Annotations;
using Lykke.Service.Balances.Core.Domain;
using Lykke.Service.Balances.Core.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.Services
{
    public class TotalBalancesService : ITotalBalancesService
    {
        private readonly IDatabase _redisDatabase;
        private readonly string _partitionKey;
        private readonly IWalletsRepository _walletsRepository;

        public TotalBalancesService([NotNull] IDatabase redisDatabase, [NotNull] string partitionKey,
            [NotNull] IWalletsRepository walletsRepository)
        {
            _redisDatabase = redisDatabase ?? throw new ArgumentNullException(nameof(redisDatabase));
            _partitionKey = partitionKey ?? throw new ArgumentNullException(nameof(partitionKey));
            _walletsRepository = walletsRepository ?? throw new ArgumentNullException(nameof(walletsRepository));
        }

        public async Task InitAsync()
        {
            var balances = await _walletsRepository.GetTotalBalancesAsync();
            await _redisDatabase.HashSetAsync(_partitionKey,
                balances.Select(x => new HashEntry(x.Key, (double)x.Value)).ToArray());
        }

        public async Task<IReadOnlyList<TotalAssetBalance>> GetTotalBalancesAsync()
        {
            var balances = await _redisDatabase.HashGetAllAsync(_partitionKey);
            return balances.Select(x => new TotalAssetBalance
            {
                AssetId = x.Name,
                Balance = (decimal)(double)x.Value
            }).ToList();
        }

        public async Task<TotalAssetBalance> GetTotalAssetBalanceAsync(string assetId)
        {
            var balance = await _redisDatabase.HashGetAsync(_partitionKey, assetId);
            if (balance.IsNull)
                return null;

            return new TotalAssetBalance
            {
                AssetId = assetId,
                Balance = (decimal)(double)balance
            };
        }

        public async Task ChangeTotalBalanceAsync(string assetId, decimal delta, long sequenceNumber)
        {
            await _redisDatabase.HashIncrementAsync(_partitionKey, assetId, (double)delta);
        }
    }
}
