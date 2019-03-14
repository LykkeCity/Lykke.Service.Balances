using Lykke.Service.Balances.Core.Domain;
using Lykke.Service.Balances.Core.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.MongoRepositories
{
    public class MongoRepository<T> : IRepository<T> where T : class, IHasId
    {
        protected readonly IMongoDatabase Database;

        public MongoRepository(IMongoDatabase database)
        {
            Database = database ?? throw new ArgumentNullException(nameof(database));

            MongoDefaults.GuidRepresentation = GuidRepresentation.Standard;
            if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
            {
                BsonClassMap.RegisterClassMap<T>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                    cm.SetIdMember(cm.GetMemberMap(x => x.Id)
                        .SetIdGenerator(GuidGenerator.Instance));
                });
            }
        }

        protected IMongoCollection<T> GetCollection()
        {
            return Database.GetCollection<T>(typeof(T).Name);
        }

        public async Task Add(T entity)
        {
            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
                await GetCollection().InsertOneAsync(entity).ConfigureAwait(false);
            }
            else
            {
                await GetCollection().ReplaceOneAsync(new BsonDocument("_id", entity.Id), entity, new UpdateOptions { IsUpsert = true });
            }
        }
    }
}
