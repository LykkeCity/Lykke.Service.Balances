using Common.Log;
using Lykke.Service.Wallets.Client;
using Xunit;

namespace Lykke.Service.Wallets.Tests.WalletsCredentials
{
    public class GetWalletsCredentialsInfo
    {
        private readonly ILog _log = new LogToMemory();
        private const string ServiceUrl = "http://client-account.lykke-service.svc.cluster.local";


        [Fact(Skip = "integration test")]
        public async void GetWalletCredentialByClientId()
        {
            var client = new WalletsClient(ServiceUrl, _log);
            var result = await (client.GetWalletCredential("35302a53-cacb-4052-b5c0-57f9c819495b"));

            Assert.NotNull(result);
            Assert.Null(result.ErrorMessage);
        }

        [Fact(Skip = "integration test")]
        public async void GetWalletCredentialHistoryByClientIdAndAssetId()
        {
            var client = new WalletsClient(ServiceUrl, _log);
            var result = await (client.GetWalletCredentialHistory("35302a53-cacb-4052-b5c0-57f9c819495b"));

            Assert.NotNull(result);
            Assert.Null(result.ErrorMessage);
        }
    }
}
