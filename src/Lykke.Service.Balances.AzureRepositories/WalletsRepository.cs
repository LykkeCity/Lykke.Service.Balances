using AzureStorage;
using JetBrains.Annotations;
using Lykke.Service.Balances.AzureRepositories.Account;
using Lykke.Service.Balances.Core.Domain.Wallets;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<IWallet>> GetTotalBalancesAsync()
        {
            return await _tableStorage.GetDataAsync(WalletEntity.GenerateTotalBalancePartitionKey());
        }

        public async Task<IWallet> GetTotalBalanceAsync(string assetId)
        {
            return await _tableStorage.GetDataAsync(WalletEntity.GenerateTotalBalancePartitionKey(), WalletEntity.GenerateRowKey(assetId));
        }

        public async Task<bool> UpdateBalanceAsync(string walletId, IWallet wallet)
        {
            var entity = WalletEntity.Create(walletId, wallet);
            var result = await _tableStorage.InsertOrReplaceAsync(entity,
                x => x.UpdateSequenceNumber == null || x.UpdateSequenceNumber.Value < wallet.UpdateSequenceNumber.Value);

            var totalBalanceEntity = await _tableStorage.GetDataAsync(WalletEntity.GenerateTotalBalancePartitionKey(), WalletEntity.GenerateRowKey(wallet.AssetId));
            if (totalBalanceEntity == null || new DateTimeOffset(DateTime.UtcNow).Subtract(totalBalanceEntity.Timestamp.ToUniversalTime()).TotalDays > 1)
            {
                var totalPartition = WalletEntity.GenerateTotalBalancePartitionKey();
                var totalRow = WalletEntity.GenerateRowKey(wallet.AssetId);
                var filter = TableQuery.CombineFilters(TableQuery.GenerateFilterCondition(nameof(WalletEntity.PartitionKey), QueryComparisons.NotEqual, totalPartition),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition(nameof(WalletEntity.RowKey), QueryComparisons.Equal, totalRow));
                var query = new TableQuery<WalletEntity>().Where(filter);
                var items = await _tableStorage.WhereAsync(query);
                decimal totalAssetBalance = 0;
                decimal totalAssetReserved = 0;
                foreach (var item in items)
                {
                    totalAssetBalance += item.Balance;
                    totalAssetReserved += item.Reserved;
                }
                await _tableStorage.InsertOrReplaceAsync(
                    new WalletEntity
                    {
                        PartitionKey = totalPartition,
                        RowKey = totalRow,
                        Balance = totalAssetBalance,
                        Reserved = totalAssetReserved,
                        UpdateSequenceNumber = wallet.UpdateSequenceNumber,
                    },
                    x => x.UpdateSequenceNumber == null || x.UpdateSequenceNumber.Value < wallet.UpdateSequenceNumber.Value);
            }

            return result;
        }
    }
}
