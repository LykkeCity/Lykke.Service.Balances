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

        public static CachedWalletModel Copy(IWallet from)
        {
            return from != null
                ? new CachedWalletModel
                {
                    Balance = from.Balance,
                    AssetId = from.AssetId,
                    Reserved = from.Reserved
                }
                : null;
        }

        public static CachedWalletModel Create(string assetId, decimal balance, decimal reserved)
        {
            return new CachedWalletModel
            {
                Balance = balance,
                AssetId = assetId,
                Reserved = reserved
            };
        }

        public void Update(decimal balance, decimal reserved)
        {
            Balance = balance;
            Reserved = reserved;
        }
    }
}
