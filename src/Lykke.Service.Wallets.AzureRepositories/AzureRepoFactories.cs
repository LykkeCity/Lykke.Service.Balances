using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.Wallets.AzureRepositories.Account;
using Lykke.Service.Wallets.AzureRepositories.Wallets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.Wallets.AzureRepositories
{
    public static class AzureRepoFactories
    {
        public static WalletsRepository CreateWalletsRepository(string connString, ILog log)
        {
            return new WalletsRepository(new AzureTableStorage<WalletEntity>(connString, "Accounts", log));
        }

        public static WalletCredentialsRepository CreateWalletsCredentialsRepository(string connString, ILog log)
        {
            return new WalletCredentialsRepository(new AzureTableStorage<WalletCredentialsEntity>(connString, "WalletCredentials", log));
        }

        public static WalletCredentialsHistoryRepository CreateWalletCredentialsHistoryRepository(string connString, ILog log)
        {
            return new WalletCredentialsHistoryRepository(new AzureTableStorage<WalletCredentialsHistoryRecord>(connString, "WalletCredentialsHistory", log));
        }
    }
}
