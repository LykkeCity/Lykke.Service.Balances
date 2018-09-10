using Lykke.Service.Balances.Core.Domain;
using MessagePack;

namespace Lykke.Service.Balances.Services.Wallet
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class CachedWalletModel : IWallet
    {
        public string AssetId { get; private set; }
        public decimal Balance { get; private set; }
        public decimal Reserved { get; private set; }

        public static CachedWalletModel Create(IWallet from)
        {
            return from != null
                ? new CachedWalletModel
                {
                    AssetId = from.AssetId,
                    Balance = from.Balance,
                    Reserved = from.Reserved
                }
                : null;
        }

        public static CachedWalletModel Create(string assetId, decimal balance, decimal reserved, long updateSequenceNumber)
        {
            return new CachedWalletModel
            {
                AssetId = assetId,
                Balance = balance,
                Reserved = reserved
            };
        }
    }
}
