using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using Lykke.Service.Balances.Core.Domain;
using Lykke.Service.Balances.Core.Services;
using MongoDB.Driver.Linq;

namespace Lykke.Service.Balances.MongoRepositories
{
    public class BalanceSnapshotRepository : MongoRepository<BalanceSnapshot>, IBalanceSnapshotRepository
    {
        private readonly TimeSpan _timeFrame;

        public BalanceSnapshotRepository(IMongoDatabase database, TimeSpan timeFrame) : base(database)
        {
            _timeFrame = timeFrame;

            CreateIndexes();
        }

        public void CreateIndexes()
        {
            GetCollection().Indexes.CreateOne(
                Builders<BalanceSnapshot>.IndexKeys.Ascending(x => x.AssetId),
                new CreateIndexOptions { Background = true }
            );
            GetCollection().Indexes.CreateOne(
                Builders<BalanceSnapshot>.IndexKeys.Ascending(x => x.WalletId).Ascending(x => x.AssetId),
                new CreateIndexOptions { Background = true }
            );

            GetCollection().Indexes.CreateOne(
                Builders<BalanceSnapshot>.IndexKeys.Ascending(x => x.Timestamp),
                new CreateIndexOptions { ExpireAfter = _timeFrame });
        }

        public Task<BalanceSnapshot> GetSnapshot(string walletId, string assetId, DateTime timestamp)
        {
            return GetCollection().AsQueryable()
                .Where(x => x.WalletId == walletId && x.AssetId == assetId && x.Timestamp <= timestamp)
                .OrderByDescending(x => x.Timestamp)
                .FirstOrDefaultAsync();
        }
    }
}
