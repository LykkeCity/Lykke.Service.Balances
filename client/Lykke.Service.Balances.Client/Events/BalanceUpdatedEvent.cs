using System;
using ProtoBuf;

namespace Lykke.Service.Balances.Client.Events
{
    [ProtoContract]
    public class BalanceUpdatedEvent
    {
        [ProtoMember(1, IsRequired = true)]
        public string WalletId { get; set; }
        [ProtoMember(2, IsRequired = true)]
        public string Balance { get; set; }
        [ProtoMember(3, IsRequired = true)]
        public string AssetId { get; set; }
        [ProtoMember(4, IsRequired = true)]
        public string Reserved { get; set; }
        [ProtoMember(5, IsRequired = true)]
        public long SequenceNumber { get; set; }
        [ProtoMember(6, IsRequired = true, DataFormat = DataFormat.WellKnown)]
        public DateTime Timestamp { get; set; }
        [ProtoMember(7, IsRequired = true)]
        public string OldBalance { get; set; }
        [ProtoMember(8, IsRequired = true)]
        public string OldReserved { get; set; }
    }
}
