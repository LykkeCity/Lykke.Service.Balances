using AzureStorage;
using Lykke.Service.Balances.AzureRepositories.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.Balances.Core.Domain.Wallets;

namespace Lykke.Service.Balances.AzureRepositories
{
    [UsedImplicitly]
    public class WalletCredentialsHistoryRepository : IWalletCredentialsHistoryRepository
    {
        private readonly INoSQLTableStorage<WalletCredentialsHistoryRecord> _tableStorage;

        public WalletCredentialsHistoryRepository(INoSQLTableStorage<WalletCredentialsHistoryRecord> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public async Task InsertHistoryRecord(IWalletCredentials oldWalletCredentials)
        {
            var entity = WalletCredentialsHistoryRecord.Create(oldWalletCredentials);
            await _tableStorage.InsertAndGenerateRowKeyAsDateTimeAsync(entity, DateTime.UtcNow);
        }

        public async Task<IEnumerable<string>> GetPrevMultisigsForUser(string clientId)
        {
            var prevWalletCreds =
                await _tableStorage.GetDataAsync(WalletCredentialsHistoryRecord.GeneratePartitionKey(clientId));

            return prevWalletCreds.Select(x => x.MultiSig);
        }
    }
}
