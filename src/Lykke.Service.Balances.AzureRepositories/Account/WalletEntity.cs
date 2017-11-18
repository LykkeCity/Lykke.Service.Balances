using Common;
using Lykke.Service.Balances.Core.Wallets;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.Balances.AzureRepositories.Account
{
    public class WalletEntity : TableEntity
    {
        public class TheWallet : IWallet
        {
            [JsonProperty("balance")]
            public double Balance { get; set; }

            [JsonProperty("asset")]
            public string AssetId { get; set; }

            [JsonProperty("reserved")]
            public double Reserved { get; set; }
        }

        public static string GeneratePartitionKey()
        {
            return "ClientBalance";
        }

        public static string GenerateRowKey(string traderId)
        {
            return traderId;
        }

        public string ClientId => RowKey;

        public string Balances { get; set; }

        internal static readonly TheWallet[] EmptyList = new TheWallet[0];

        internal TheWallet[] Get()
        {
            if (string.IsNullOrEmpty(Balances))
                return EmptyList;

            return Balances.DeserializeJson(() => EmptyList);
        }
        public static WalletEntity Create(string clientId)
        {
            return new WalletEntity
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(clientId),

            };
        }
    }
}
