using Newtonsoft.Json;

namespace Lykke.Service.Balances.Core.Domain.Wallets
{
    public interface IWallet
    {
        double Balance { get; }
        double Reserved { get; }
        string AssetId { get; }
    }
}
