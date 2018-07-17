namespace Lykke.Job.Balances.RabbitSubscribers
{
    public class BalanceUpdatedEvent
    {
        public string WalletId { get; set; }
        public string Balance { get; set; }
        public string AssetId { get; set; }
        public string Reserved { get; set; }
        public long SequenceNumber { get; set; }
    }
}
