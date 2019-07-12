using System;
using Lykke.Service.Balances.Core.Domain;
using ProtoBuf;

namespace Lykke.Service.Balances.Services.Wallet
{
    [ProtoContract]
    public class CachedWalletModel : IWallet
    {
        [ProtoMember(1)]
        public string AssetId { get; private set; }
        [ProtoMember(2)]
        public decimal Balance { get; private set; }
        [ProtoMember(3)]
        public decimal Reserved { get; private set; }
        [ProtoMember(4)]
        public DateTime? UpdatedAt { get; private set; }

        public static CachedWalletModel Create(IWallet from)
        {
            return from != null
                ? new CachedWalletModel
                {
                    AssetId = from.AssetId,
                    Balance = from.Balance,
                    Reserved = from.Reserved,
                    UpdatedAt = from.UpdatedAt
                }
                : null;
        }

        public static CachedWalletModel Create(string assetId, decimal balance, decimal reserved, DateTime? timestamp)
        {
            return new CachedWalletModel
            {
                AssetId = assetId,
                Balance = balance,
                Reserved = reserved,
                UpdatedAt = timestamp
            };
        }
    }
}
