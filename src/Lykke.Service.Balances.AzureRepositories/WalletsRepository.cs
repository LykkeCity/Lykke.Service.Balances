using AzureStorage;
using Lykke.Service.Balances.AzureRepositories.Account;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.Balances.Core.Domain.Wallets;

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

        public async Task UpdateBalanceAsync(string walletId, List<IWallet> wallets)
        {
            var entities = wallets.Select(item => WalletEntity.Create(walletId, item));
            await _tableStorage.InsertOrMergeBatchAsync(entities);
        }
    }
}
