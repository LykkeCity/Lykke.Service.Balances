using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Lykke.Service.Balances.Core.Domain.Wallets;

namespace Lykke.Service.Balances.AzureRepositories.Account
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateAlways)]
    public class WalletEntity : AzureTableEntity, IWallet
    {
        public string WalletId => PartitionKey;
        public string AssetId => RowKey;
        public decimal Balance { get; set; }
        public decimal Reserved { get; set; }
        
        internal static string GeneratePartitionKey(string walletId) => walletId;
        internal static string GenerateTotalBalancePartitionKey() => "TotalBalance";
        internal static string GenerateRowKey(string assetId) => assetId;

        internal static WalletEntity Create(string walletId, IWallet src)
        {
            return new WalletEntity
            {
                PartitionKey = GeneratePartitionKey(walletId),
                RowKey = GenerateRowKey(src.AssetId),
                Balance = src.Balance,
                Reserved = src.Reserved
            };
        }
        
        internal static WalletEntity CreateTotal(IWallet src)
        {
            return new WalletEntity
            {
                PartitionKey = GenerateTotalBalancePartitionKey(),
                RowKey = GenerateRowKey(src.AssetId),
                Balance = src.Balance,
                Reserved = src.Reserved
            };
        }
        
        internal static WalletEntity Create(string walletId, string assetId, decimal balance, decimal reserved)
        {
            return new WalletEntity
            {
                PartitionKey = GeneratePartitionKey(walletId),
                RowKey = GenerateRowKey(assetId),
                Balance = balance,
                Reserved = reserved
            };
        }
    }
}
