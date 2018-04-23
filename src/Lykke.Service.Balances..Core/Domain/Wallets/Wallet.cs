namespace Lykke.Service.Balances.Core.Domain.Wallets
{
    public class Wallet : IWallet
    {
        public decimal Balance { get; set; }
        public decimal Reserved { get; set; }
        public string AssetId { get; set; }

        public static IWallet Create(IWallet src)
        {
            return new Wallet
            {
                AssetId = src.AssetId,
                Balance = src.Balance,
                Reserved = src.Reserved
            };
        }
    }
}
