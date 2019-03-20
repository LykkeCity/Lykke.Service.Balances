using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.Balances.Core.Domain;
using Lykke.Service.Balances.Core.Services;
using MongoDB.Driver.Linq;
using Enumerable = System.Linq.Enumerable;

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

            //todo: GetCollection().Indexes.DropOne("timeframe");
            GetCollection().Indexes.CreateOne(
                Builders<BalanceSnapshot>.IndexKeys.Ascending(x => x.Timestamp),
                new CreateIndexOptions { ExpireAfter = _timeFrame, Background = true, Name = "timeframe"});
        }

        public Task<BalanceSnapshot> GetSnapshot(string walletId, string assetId, DateTime timestamp)
        {
            return GetCollection().AsQueryable()
                .Where(x => x.WalletId == walletId && x.AssetId == assetId && x.Timestamp <= timestamp)
                .OrderByDescending(x => x.Timestamp)
                .FirstOrDefaultAsync();
        }

        public async Task<List<BalanceSnapshot>> GetSnapshots(string assetId, DateTime timestamp)
        {
            // todo: implement
            //return GetCollection().AsQueryable()
            //    .Where(x => x.AssetId == assetId && x.Timestamp <= timestamp)
            //    .GroupBy(x => x.WalletId)
            //    .Select(x => Enumerable.First(Enumerable.OrderByDescending(x, z => z.Timestamp)))
            //    .Where(x => x.Balance > 0)
            //    .ToListAsync();

            return new List<BalanceSnapshot>
            {
                new BalanceSnapshot
                {
                    Balance = 666,
                    WalletId = "fake wallet id"
                }
            };
        }
    }
}
