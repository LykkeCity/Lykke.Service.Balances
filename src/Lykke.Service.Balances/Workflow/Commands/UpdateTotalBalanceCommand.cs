using ProtoBuf;

namespace Lykke.Service.Balances.Workflow.Commands
{
    [ProtoContract]
    public class UpdateTotalBalanceCommand
    {
        [ProtoMember(1)]
        public long SequenceNumber { get; set; }
        [ProtoMember(2)]
        public string AssetId { get; set; }
        [ProtoMember(3)]
        public decimal DeltaBalance { get; set; }
    }
}
