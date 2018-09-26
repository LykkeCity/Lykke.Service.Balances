using AzureStorage;
using JetBrains.Annotations;
using Lykke.Service.Balances.Core.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.AzureRepositories
{
    [UsedImplicitly]
    public class WalletsRepository : IWalletsRepository
    {
        private readonly INoSQLTableStorage<WalletEntity> _tableStorage;

        public WalletsRepository(INoSQLTableStorage<WalletEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task<IEnumerable<IWallet>> GetAsync(string walletId)
        {
            return await _tableStorage.GetDataAsync(WalletEntity.GeneratePartitionKey(walletId));
        }

        public async Task<IWallet> GetAsync(string walletId, string assetId)
        {
            return await _tableStorage.GetDataAsync(WalletEntity.GeneratePartitionKey(walletId), WalletEntity.GenerateRowKey(assetId));
        }

        public Task<bool> UpdateBalanceAsync(string walletId, IWallet wallet, long updateSequenceNumber)
        {
            var entity = WalletEntity.Create(walletId, wallet, updateSequenceNumber);
            return _tableStorage.InsertOrReplaceAsync(entity,
                x => x.UpdateSequenceNumber == null || x.UpdateSequenceNumber.Value < updateSequenceNumber);
        }

        private const decimal TooBigValue = (long)1 << 60; // 1 * 10^18
        public async Task<Dictionary<string, decimal>> GetTotalBalancesAsync()
        {
            var groupedBalances = new List<Dictionary<string, decimal>>();
            await _tableStorage.GetDataByChunksAsync(entities =>
            {
                var balances = entities
                    .Where(x => x.AssetId != "TotalBalance" && x.Balance > 0m && x.Balance < TooBigValue)
                    .GroupBy(g => g.AssetId)
                    .ToDictionary(d => d.Key, g => g.Sum(wallet => wallet.Balance));
                groupedBalances.Add(balances);
            });
            return groupedBalances.SelectMany(dict => dict)
                .ToLookup(pair => pair.Key, pair => pair.Value)
                .ToDictionary(group => group.Key, group => group.Sum());
        }
    }
}
