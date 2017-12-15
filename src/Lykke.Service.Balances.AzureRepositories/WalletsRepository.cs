using AzureStorage;
using Lykke.Service.Balances.AzureRepositories.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Service.Balances.Core.Domain.Wallets;

namespace Lykke.Service.Balances.AzureRepositories
{
    [UsedImplicitly]
    public class WalletsRepository : IWalletsRepository
    {
        private readonly ILog _log;
        private readonly INoSQLTableStorage<WalletEntity> _tableStorage;

        public WalletsRepository(ILog log, INoSQLTableStorage<WalletEntity> tableStorage)
        {
            _log = log;
            _tableStorage = tableStorage;
        }

        public async Task<IEnumerable<IWallet>> GetAsync(string traderId)
        {
#if DEBUG
            await _log.WriteInfoAsync("Get wallet from the storage", traderId, string.Empty);
#endif

            var partitionKey = WalletEntity.GeneratePartitionKey();
            var rowKey = WalletEntity.GenerateRowKey(traderId);
            var entity = await _tableStorage.GetDataAsync(partitionKey, rowKey);

            return entity == null
                ? WalletEntity.EmptyList
                : entity.Get();
        }

        public async Task<Dictionary<string, double>> GetTotalBalancesAsync()
        {
            var result = new Dictionary<string, double>();

            await _tableStorage.GetDataByChunksAsync(entities =>
            {
                foreach (var walletEntity in entities)
                    foreach (var balances in walletEntity.Get())
                    {
                        if (!result.ContainsKey(balances.AssetId))
                            result.Add(balances.AssetId, balances.Balance);
                        else
                            result[balances.AssetId] += balances.Balance;
                    }
            });

            return result;
        }

        public async Task GetWalletsByChunkAsync(Func<IEnumerable<KeyValuePair<string, IEnumerable<IWallet>>>, Task> chunkCallback)
        {

            await _tableStorage.GetDataByChunksAsync(async chunk =>
            {
                var yeldResult = new List<KeyValuePair<string, IEnumerable<IWallet>>>();

                foreach (var walletEntity in chunk)
                {
                    var wallets = walletEntity.Get().Where(itm => itm.Balance != 0).ToArray();
                    if (wallets.Length > 0)
                        yeldResult.Add(new KeyValuePair<string, IEnumerable<IWallet>>(walletEntity.ClientId, wallets));
                }

                if (yeldResult.Count > 0)
                {
                    await chunkCallback(yeldResult);
                    yeldResult.Clear();
                }

            });
        }
    }

}
