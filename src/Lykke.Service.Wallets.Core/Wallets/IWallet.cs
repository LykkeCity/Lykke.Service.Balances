namespace Lykke.Service.Wallets.Core.Wallets
{
    public interface IWallet
    {
        double Balance { get; }
        string AssetId { get; }
    }
}
