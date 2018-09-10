﻿using Lykke.Service.Balances.Core.Domain;
using Lykke.Service.Balances.Core.Services;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.Services
{
    public class TotalBalancesService : ITotalBalancesService
    {
        private readonly IDatabase _redisDatabase;
        private readonly string _partitionKey;

        public TotalBalancesService(IDatabase redisDatabase, string partitionKey)
        {
            _redisDatabase = redisDatabase;
            _partitionKey = partitionKey;
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
