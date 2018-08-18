using System;
using AzureStorage;
using JetBrains.Annotations;
using Lykke.Service.Balances.AzureRepositories.Account;
using Lykke.Service.Balances.Core.Domain.Wallets;
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

        public async Task<IWallet> GetTotalBalanceAsync(string assetId)
        {
            var assetPartition = WalletEntity.GenerateAssetPartitionKey(assetId);
            var items = await _tableStorage.GetDataAsync(assetPartition);
            decimal totalAssetBalance = 0;
            decimal totalAssetReserved = 0;
            foreach (var item in items)
            {
                totalAssetBalance += item.Balance;
                totalAssetReserved += item.Reserved;
            }
            return new WalletEntity
            {
                RowKey = assetId,
                Balance = totalAssetBalance,
                Reserved = totalAssetReserved
            };
        }

        public async Task<bool> UpdateBalanceAsync(string walletId, IWallet wallet)
        {
            var entity = WalletEntity.Create(walletId, wallet);
            var result = await _tableStorage.InsertOrReplaceAsync(entity,
                x => x.UpdateSequenceNumber == null || x.UpdateSequenceNumber.Value < wallet.UpdateSequenceNumber.Value);

            if (wallet.Balance == 0 && wallet.Reserved == 0)
                await _tableStorage.DeleteIfExistAsync(
                    WalletEntity.GenerateAssetPartitionKey(wallet.AssetId),
                    WalletEntity.GenerateAssetRowKey(walletId),
                    x => x.UpdateSequenceNumber == null || x.UpdateSequenceNumber.Value < wallet.UpdateSequenceNumber.Value);
            else
                await _tableStorage.InsertOrReplaceAsync(entity.CopyForAsset(),
                    x => x.UpdateSequenceNumber == null || x.UpdateSequenceNumber.Value < wallet.UpdateSequenceNumber.Value);

            return result;
        }

        //TODO remove this in next version
        public async Task InitAssetsWalletsAsync(HashSet<string> assetIds)
        {
            await _tableStorage.GetDataByChunksAsync(async chunk =>
            {
                foreach (var entity in chunk)
                {
                    if (!assetIds.Contains(entity.RowKey))
                        continue;

                    var assetWallet = entity.CopyForAsset();
                    await _tableStorage.TryInsertAsync(assetWallet);
                }
            });
        }
    }
}
