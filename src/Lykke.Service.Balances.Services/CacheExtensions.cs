﻿using System;
using System.IO;
using System.Threading.Tasks;
using Common.Log;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.Caching.Distributed;

namespace Lykke.Service.Balances.Services
{
    internal static class CacheExtensions
    {
        // TODO: Add update predicate
        // TODO: Make atomic
        public static async Task<T> TryGetFromCacheAsync<T>(this IDistributedCache cache, 
            string key,
            Func<Task<T>> getRecordFunc, 
            TimeSpan? absoluteExpiration = null, 
            TimeSpan? slidingExpiration = null,
            ILog log = null)
        {
            var record = await cache.TryGetFromCacheAsync<T>(key);

            if (record == null)
            {
                record = await getRecordFunc();
                await cache.TryUpdateCacheAsync(key, record, absoluteExpiration, slidingExpiration, log);
            }

            return record;
        }

        public static async Task<T> TryGetFromCacheAsync<T>(this IDistributedCache cache, 
            string key,
            ILog log = null)
        {
            byte[] value;
            try
            {
                value = await cache.GetAsync(key);
            }
            catch (StackExchange.Redis.RedisConnectionException ex)
            {
                // todo: use new logging contracts
                log?.WriteWarning("", "", "Redis cache is not available", ex);
                // ignoring the errors
                return default(T);
            }

            if (value != null)
            {
                using (var stream = new MemoryStream(value))
                {
                    return MessagePackSerializer.Deserialize<T>(stream, StandardResolverAllowPrivate.Instance);
                }
            }

            return default(T);
        }

        public static async Task<bool> TryUpdateCacheAsync<T>(this IDistributedCache cache, 
            string key,
            T record, 
            TimeSpan? absoluteExpiration = null, 
            TimeSpan? slidingExpiration = null,
            ILog log = null)
        {
            var value = MessagePackSerializer.Serialize(record);

            try
            {
                await cache.SetAsync(key, value, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = absoluteExpiration,
                    SlidingExpiration = slidingExpiration
                });
                return true;
            }
            catch (StackExchange.Redis.RedisConnectionException ex)
            {
                // todo: use new logging contracts
                log?.WriteWarning("", "", "Redis cache is not available", ex);
                // ignoring the errors
                return false;
            }
        }
    }
}