using Common.Log;
using Lykke.Common.Log;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.Services
{
    internal static class RedisCacheExtensions
    {
        public static async Task<T> TryHashGetAsync<T>(this IDatabase cache,
            string key,
            string field,
            Func<Task<T>> getRecordFunc,
            Func<Task> reloadAllAction,
            TimeSpan? cacheExpiration,
            ILog log = null)
        {
            var record = await cache.TryHashGetAsync<T>(key, field, log);

            if (record == null)
            {
                record = await getRecordFunc();
                if (record != null)
                {
                    if (!await cache.KeyExistsAsync(key) && reloadAllAction != null)
                    {
                        await reloadAllAction();
                    }
                    else
                    {
                        await cache.TryHashSetAsync(key, field, record, cacheExpiration, log);
                    }
                }
            }

            return record;
        }

        public static async Task<T> TryHashGetAsync<T>(this IDatabase cache,
            string key,
            string field,
            ILog log = null)
        {
            try
            {
                var record = await cache.HashGetAsync(key, field);
                if (record.HasValue)
                {
                    return CacheSerializer.Deserialize<T>(record);
                }
            }
            catch (RedisConnectionException ex)
            {
                log?.Warning("Redis cache is not available", ex);
            }

            return default(T);
        }

        public static async Task<bool> TryHashSetAsync<T>(this IDatabase cache,
            string key,
            string field,
            T record,
            TimeSpan? cacheExpiration = null,
            ILog log = null)
        {
            try
            {
                await cache.HashSetAsync(key, field, CacheSerializer.Serialize(record));
                if (cacheExpiration.HasValue)
                {
                    await cache.KeyExpireAsync(key, cacheExpiration);
                }

                return true;
            }
            catch (RedisConnectionException ex)
            {
                log?.Warning("Redis cache is not available", ex);
                // ignoring the errors
                return false;
            }
        }
    }
}