using Lykke.Service.Balances.Core.Domain.Wallets;
using MessagePack;

namespace Lykke.Service.Balances.Services.Wallet.CacheModels
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class CachedWalletModel : IWallet
    {
        public double Balance { get; private set; }
        public string AssetId { get; private set; }
        public double Reserved { get; private set; }

        public static CachedWalletModel Copy(IWallet from)
        {
            return new CachedWalletModel
            {
                Balance = from.Balance,
                AssetId = from.AssetId,
                Reserved = from.Reserved
            };
        }

        public static CachedWalletModel Create(string assetId, double balance, double reserved)
        {
            return new CachedWalletModel
            {
                Balance = balance,
                AssetId = assetId,
                Reserved = reserved
            };
        }

        public void Update(double balance, double reserved)
        {
            Balance = balance;
            Reserved = reserved;
        }
    }
}