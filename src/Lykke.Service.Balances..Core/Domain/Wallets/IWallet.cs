namespace Lykke.Service.Balances.Core.Domain.Wallets
{
    public interface IWallet
    {
        decimal Balance { get; }
        decimal Reserved { get; }
        string AssetId { get; }
        long? UpdateSequenceNumber { get; }

        IWallet Update(decimal newBalance, decimal newReserved);
    }
}
