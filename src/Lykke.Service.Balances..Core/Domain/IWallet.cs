namespace Lykke.Service.Balances.Core.Domain
{
    public interface IWallet
    {
        string AssetId { get; }
        decimal Balance { get; }
        decimal Reserved { get; }
    }
}
