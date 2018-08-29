using ProtoBuf;

namespace Lykke.Service.Balances.Workflow.Events
{
    [ProtoContract]
    public class BalanceUpdatedEvent
    {
        [ProtoMember(1)]
        public string WalletId { get; set; }
        [ProtoMember(2)]
        public string Balance { get; set; }
        [ProtoMember(3)]
        public string AssetId { get; set; }
        [ProtoMember(4)]
        public string Reserved { get; set; }
        [ProtoMember(5)]
        public long SequenceNumber { get; set; }
    }
}
