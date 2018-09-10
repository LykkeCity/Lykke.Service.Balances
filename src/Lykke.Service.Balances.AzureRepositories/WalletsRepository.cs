using AzureStorage;
using JetBrains.Annotations;
using Lykke.Service.Balances.Core.Domain;
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

        public Task<bool> UpdateBalanceAsync(string walletId, IWallet wallet, long updateSequenceNumber)
        {
            var entity = WalletEntity.Create(walletId, wallet, updateSequenceNumber);
            return _tableStorage.InsertOrReplaceAsync(entity,
                x => x.UpdateSequenceNumber == null || x.UpdateSequenceNumber.Value < updateSequenceNumber);
        }
    }
}
