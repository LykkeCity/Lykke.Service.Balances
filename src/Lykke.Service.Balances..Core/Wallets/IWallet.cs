namespace Lykke.Service.Balances.Core.Wallets
{
    public interface IWallet
    {
        double Balance { get; }
        double Reserved { get; }
        string AssetId { get; }
    }
}
