namespace Lykke.Service.Balances.Core.Wallets
{
    public interface IWallet
    {
        double Balance { get; }
        string AssetId { get; }
    }
}
