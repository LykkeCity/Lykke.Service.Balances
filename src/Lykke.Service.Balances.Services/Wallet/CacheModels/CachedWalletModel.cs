using Lykke.Service.Balances.Core.Domain.Wallets;
using MessagePack;

namespace Lykke.Service.Balances.Services.Wallet.CacheModels
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class CachedWalletModel : IWallet
    {
        public decimal Balance { get; private set; }
        public string AssetId { get; private set; }
        public decimal Reserved { get; private set; }
        public long? UpdateSequenceNumber { get; private set; }

        public static CachedWalletModel Copy(IWallet from)
        {
            return from != null
                ? new CachedWalletModel
                {
                    Balance = from.Balance,
                    AssetId = from.AssetId,
                    Reserved = from.Reserved,
                    UpdateSequenceNumber = from.UpdateSequenceNumber
                }
                : null;
        }

        public static CachedWalletModel Create(string assetId, decimal balance, decimal reserved, long updateSequenceNumber)
        {
            return new CachedWalletModel
            {
                Balance = balance,
                AssetId = assetId,
                Reserved = reserved,
                UpdateSequenceNumber = updateSequenceNumber
            };
        }
    }
}
