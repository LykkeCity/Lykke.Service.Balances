using AzureStorage.Tables;
using Common.Log;
using Lykke.Service.Wallets.AzureRepositories.Account;
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
    }
}
