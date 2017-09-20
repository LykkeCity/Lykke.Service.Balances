using Common.Log;
using Lykke.Service.Wallets.Client;
using Xunit;

namespace Lykke.Service.Wallets.Tests.ClientBalances
{
    public class GetClientBalances
    {
        private readonly ILog _log = new LogToMemory();
        private const string ServiceUrl = "http://client-account.lykke-service.svc.cluster.local";


        [Fact]
        public void GetClientBalancesByClientId()
        {
            var client = new WalletsClient(ServiceUrl, _log);
            var result = client.GetClientBalances("35302a53-cacb-4052-b5c0-57f9c819495b");

            Assert.NotNull(result);
        }

        [Fact]
        public void GetClientBalancesByClientIdAndAssetId()
        {
            var client = new WalletsClient(ServiceUrl, _log);
            var result = client.GetClientBalanceByAssetId(new Client.AutorestClient.Models.ClientBalanceByAssetIdModel() { ClientId = "35302a53-cacb-4052-b5c0-57f9c819495b", AssetId = "USD" });

            Assert.NotNull(result);
        }
    }
}
