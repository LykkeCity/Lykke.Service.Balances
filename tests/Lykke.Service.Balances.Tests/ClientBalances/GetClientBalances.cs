using Lykke.Service.Balances.AutorestClient.Models;
using Lykke.Service.Balances.Client;
using Xunit;

namespace Lykke.Service.Balances.Tests.ClientBalances
{
    public class GetClientBalances
    {
        private const string ServiceUrl = "http://client-account.lykke-service.svc.cluster.local";

        [Fact(Skip = "integration test")]
        public void GetClientBalancesByClientId()
        {
            var client = new BalancesClient(ServiceUrl);
            var result = client.GetClientBalances("35302a53-cacb-4052-b5c0-57f9c819495b");

            Assert.NotNull(result);
        }

        [Fact(Skip = "integration test")]
        public void GetClientBalancesByClientIdAndAssetId()
        {
            var client = new BalancesClient(ServiceUrl);
            var result = client.GetClientBalanceByAssetId( new ClientBalanceByAssetIdModel { ClientId = "35302a53-cacb-4052-b5c0-57f9c819495b", AssetId = "USD" });

            Assert.NotNull(result);
        }
    }
}
