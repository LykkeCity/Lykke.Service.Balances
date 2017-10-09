using AzureStorage;
using Lykke.Service.Balances.AzureRepositories.Wallets;
using Lykke.Service.Balances.Core.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.AzureRepositories
{
    public class WalletCredentialsRepository : IWalletCredentialsRepository
    {
        private readonly INoSQLTableStorage<WalletCredentialsEntity> _tableStorage;

        public WalletCredentialsRepository(INoSQLTableStorage<WalletCredentialsEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public Task SaveAsync(IWalletCredentials walletCredentials)
        {
            var newByClientEntity = WalletCredentialsEntity.ByClientId.CreateNew(walletCredentials);
            var newByMultisigEntity = WalletCredentialsEntity.ByMultisig.CreateNew(walletCredentials);
            var newByColoredEntity = WalletCredentialsEntity.ByColoredMultisig.CreateNew(walletCredentials);

            var insertByEthTask = Task.CompletedTask;
            if (!string.IsNullOrEmpty(walletCredentials.EthConversionWalletAddress))
            {
                var newByEthWalletEntity = WalletCredentialsEntity.ByEthContract.CreateNew(walletCredentials);
                insertByEthTask = _tableStorage.InsertAsync(newByEthWalletEntity);
            }

            var insertBySolarTask = Task.CompletedTask;
            if (!string.IsNullOrEmpty(walletCredentials.SolarCoinWalletAddress))
            {
                var newBySolarWalletEntity = WalletCredentialsEntity.BySolarCoinWallet.CreateNew(walletCredentials);
                insertBySolarTask = _tableStorage.InsertAsync(newBySolarWalletEntity);
            }

            var insertByChronoBankTask = Task.CompletedTask;
            if (!string.IsNullOrEmpty(walletCredentials.ChronoBankContract))
            {
                var newByChronoContractEntity = WalletCredentialsEntity.ByChronoBankContract.CreateNew(walletCredentials);
                insertByChronoBankTask = _tableStorage.InsertAsync(newByChronoContractEntity);
            }

            var insertByQuantaTask = Task.CompletedTask;
            if (!string.IsNullOrEmpty(walletCredentials.QuantaContract))
            {
                var newByQuantaContractEntity = WalletCredentialsEntity.ByQuantaContract.CreateNew(walletCredentials);
                insertByQuantaTask = _tableStorage.InsertAsync(newByQuantaContractEntity);
            }

            return Task.WhenAll(
                _tableStorage.InsertAsync(newByClientEntity),
                _tableStorage.InsertAsync(newByMultisigEntity),
                _tableStorage.InsertAsync(newByColoredEntity),
                insertByEthTask,
                insertBySolarTask,
                insertByChronoBankTask,
                insertByQuantaTask
                );
        }

        public Task MergeAsync(IWalletCredentials walletCredentials)
        {
            var newByClientEntity = WalletCredentialsEntity.ByClientId.CreateNew(walletCredentials);
            var newByMultisigEntity = WalletCredentialsEntity.ByMultisig.CreateNew(walletCredentials);
            var newByColoredEntity = WalletCredentialsEntity.ByColoredMultisig.CreateNew(walletCredentials);

            var insertByEthTask = Task.CompletedTask;
            if (!string.IsNullOrEmpty(walletCredentials.EthConversionWalletAddress))
            {
                var newByEthWalletEntity = WalletCredentialsEntity.ByEthContract.CreateNew(walletCredentials);
                insertByEthTask = _tableStorage.InsertOrMergeAsync(newByEthWalletEntity);
            }

            var insertBySolarTask = Task.CompletedTask;
            if (!string.IsNullOrEmpty(walletCredentials.SolarCoinWalletAddress))
            {
                var newBySolarWalletEntity = WalletCredentialsEntity.BySolarCoinWallet.CreateNew(walletCredentials);
                insertBySolarTask = _tableStorage.InsertOrMergeAsync(newBySolarWalletEntity);
            }

            var insertByChronoBankTask = Task.CompletedTask;
            if (!string.IsNullOrEmpty(walletCredentials.ChronoBankContract))
            {
                var newByChronoContractEntity = WalletCredentialsEntity.ByChronoBankContract.CreateNew(walletCredentials);
                insertByChronoBankTask = _tableStorage.InsertOrMergeAsync(newByChronoContractEntity);
            }

            var insertByQuantaTask = Task.CompletedTask;
            if (!string.IsNullOrEmpty(walletCredentials.QuantaContract))
            {
                var newByQuantaEntity = WalletCredentialsEntity.ByQuantaContract.CreateNew(walletCredentials);
                insertByQuantaTask = _tableStorage.InsertOrMergeAsync(newByQuantaEntity);
            }

            return Task.WhenAll(
                _tableStorage.InsertOrMergeAsync(newByClientEntity),
                _tableStorage.InsertOrMergeAsync(newByMultisigEntity),
                _tableStorage.InsertOrMergeAsync(newByColoredEntity),
                insertByEthTask,
                insertBySolarTask,
                insertByChronoBankTask,
                insertByQuantaTask);
        }

        public async Task<IWalletCredentials> GetAsync(string clientId)
        {
            var partitionKey = WalletCredentialsEntity.ByClientId.GeneratePartitionKey();
            var rowKey = WalletCredentialsEntity.ByClientId.GenerateRowKey(clientId);

            var entity = await _tableStorage.GetDataAsync(partitionKey, rowKey);

            if (entity == null)
                return null;

            return string.IsNullOrEmpty(entity.MultiSig) ? null : entity;
        }

        public async Task<IWalletCredentials> GetByEthConversionWalletAsync(string ethWallet)
        {
            var partitionKey = WalletCredentialsEntity.ByEthContract.GeneratePartitionKey();
            var rowKey = WalletCredentialsEntity.ByEthContract.GenerateRowKey(ethWallet);

            return await _tableStorage.GetDataAsync(partitionKey, rowKey);
        }

        public async Task<IWalletCredentials> GetBySolarCoinWalletAsync(string address)
        {
            var partitionKey = WalletCredentialsEntity.BySolarCoinWallet.GeneratePartitionKey();
            var rowKey = WalletCredentialsEntity.BySolarCoinWallet.GenerateRowKey(address);

            return await _tableStorage.GetDataAsync(partitionKey, rowKey);
        }

        public async Task<IWalletCredentials> GetByChronoBankContractAsync(string contract)
        {
            var partitionKey = WalletCredentialsEntity.ByChronoBankContract.GeneratePartitionKey();
            var rowKey = WalletCredentialsEntity.ByChronoBankContract.GenerateRowKey(contract);

            return await _tableStorage.GetDataAsync(partitionKey, rowKey);
        }

        public async Task<string> GetClientIdByMultisig(string multisig)
        {
            var partitionKey = WalletCredentialsEntity.ByMultisig.GeneratePartitionKey();
            var rowKey = WalletCredentialsEntity.ByMultisig.GenerateRowKey(multisig);

            var entity = await _tableStorage.GetDataAsync(partitionKey, rowKey);

            if (entity == null)
            {
                //try to find by colored
                partitionKey = WalletCredentialsEntity.ByColoredMultisig.GeneratePartitionKey();
                rowKey = WalletCredentialsEntity.ByColoredMultisig.GenerateRowKey(multisig);
                entity = await _tableStorage.GetDataAsync(partitionKey, rowKey);
            }

            return entity?.ClientId;
        }

        public async Task SetPreventTxDetection(string clientId, bool value)
        {
            var partitionKeyByClient = WalletCredentialsEntity.ByClientId.GeneratePartitionKey();
            var rowKeyByClient = WalletCredentialsEntity.ByClientId.GenerateRowKey(clientId);

            var currentRecord = await _tableStorage.GetDataAsync(partitionKeyByClient, rowKeyByClient);
            var changedRecord = WalletCredentials.Create(currentRecord);
            changedRecord.PreventTxDetection = value;

            await MergeAsync(changedRecord);
        }

        public async Task SetEncodedPrivateKey(string clientId, string encodedPrivateKey)
        {
            var partitionKeyByClient = WalletCredentialsEntity.ByClientId.GeneratePartitionKey();
            var rowKeyByClient = WalletCredentialsEntity.ByClientId.GenerateRowKey(clientId);

            var currentRecord = await _tableStorage.GetDataAsync(partitionKeyByClient, rowKeyByClient);
            var changedRecord = WalletCredentials.Create(currentRecord);
            changedRecord.EncodedPrivateKey = encodedPrivateKey;

            await MergeAsync(changedRecord);
        }

        public async Task SetEthConversionWallet(string clientId, string contract)
        {
            var partitionKeyByClient = WalletCredentialsEntity.ByClientId.GeneratePartitionKey();
            var rowKeyByClient = WalletCredentialsEntity.ByClientId.GenerateRowKey(clientId);

            var currentRecord = await _tableStorage.GetDataAsync(partitionKeyByClient, rowKeyByClient);

            if (string.IsNullOrEmpty(currentRecord.EthConversionWalletAddress))
            {
                var changedRecord = WalletCredentials.Create(currentRecord);
                changedRecord.EthConversionWalletAddress = contract;

                var newByEthWalletEntity = WalletCredentialsEntity.ByEthContract.CreateNew(changedRecord);
                await _tableStorage.InsertOrReplaceAsync(newByEthWalletEntity);

                await MergeAsync(changedRecord);
            }
        }

        public async Task SetEthFieldsWallet(string clientId, string contract, string address, string pubKey)
        {
            var partitionKeyByClient = WalletCredentialsEntity.ByClientId.GeneratePartitionKey();
            var rowKeyByClient = WalletCredentialsEntity.ByClientId.GenerateRowKey(clientId);

            var currentRecord = await _tableStorage.GetDataAsync(partitionKeyByClient, rowKeyByClient);

            var changedRecord = WalletCredentials.Create(currentRecord);
            changedRecord.EthConversionWalletAddress = contract;
            changedRecord.EthPublicKey = pubKey;
            changedRecord.EthAddress = address;

            if (string.IsNullOrEmpty(currentRecord.EthConversionWalletAddress))
            {
                var newByEthWalletEntity = WalletCredentialsEntity.ByEthContract.CreateNew(changedRecord);
                await _tableStorage.InsertOrReplaceAsync(newByEthWalletEntity);
            }

            await MergeAsync(changedRecord);
        }

        public async Task SetSolarCoinWallet(string clientId, string address)
        {
            var partitionKeyByClient = WalletCredentialsEntity.ByClientId.GeneratePartitionKey();
            var rowKeyByClient = WalletCredentialsEntity.ByClientId.GenerateRowKey(clientId);

            var currentRecord = await _tableStorage.GetDataAsync(partitionKeyByClient, rowKeyByClient);

            if (string.IsNullOrEmpty(currentRecord.SolarCoinWalletAddress))
            {
                var changedRecord = WalletCredentials.Create(currentRecord);
                changedRecord.SolarCoinWalletAddress = address;
                await MergeAsync(changedRecord);
            }
        }

        public async Task SetChronoBankContract(string clientId, string contract)
        {
            var partitionKeyByClient = WalletCredentialsEntity.ByClientId.GeneratePartitionKey();
            var rowKeyByClient = WalletCredentialsEntity.ByClientId.GenerateRowKey(clientId);

            var currentRecord = await _tableStorage.GetDataAsync(partitionKeyByClient, rowKeyByClient);

            if (string.IsNullOrEmpty(currentRecord.ChronoBankContract))
            {
                var changedRecord = WalletCredentials.Create(currentRecord);
                changedRecord.ChronoBankContract = contract;
                await MergeAsync(changedRecord);
            }
        }

        public async Task<IWalletCredentials> ScanAndFind(Func<IWalletCredentials, bool> callBack)
        {
            var partitionKey = WalletCredentialsEntity.ByClientId.GeneratePartitionKey();

            return await _tableStorage.FirstOrNullViaScanAsync(partitionKey, chunk =>
            { return chunk.FirstOrDefault(item => callBack(item)); });
        }

        public Task ScanAllAsync(Func<IEnumerable<IWalletCredentials>, Task> chunk)
        {
            var partitionKey = WalletCredentialsEntity.ByClientId.GeneratePartitionKey();

            return _tableStorage.ScanDataAsync(partitionKey, chunk);

        }

        public async Task<IWalletCredentials> GetByQuantaContractAsync(string contract)
        {
            var partitionKey = WalletCredentialsEntity.ByQuantaContract.GeneratePartitionKey();
            var rowKey = WalletCredentialsEntity.ByQuantaContract.GenerateRowKey(contract);

            return await _tableStorage.GetDataAsync(partitionKey, rowKey);
        }

        public async Task SetQuantaContract(string clientId, string contract)
        {
            var partitionKeyByClient = WalletCredentialsEntity.ByClientId.GeneratePartitionKey();
            var rowKeyByClient = WalletCredentialsEntity.ByClientId.GenerateRowKey(clientId);

            var currentRecord = await _tableStorage.GetDataAsync(partitionKeyByClient, rowKeyByClient);

            if (string.IsNullOrEmpty(currentRecord.QuantaContract))
            {
                var changedRecord = WalletCredentials.Create(currentRecord);
                changedRecord.QuantaContract = contract;
                await MergeAsync(changedRecord);
            }
        }
    }
}
