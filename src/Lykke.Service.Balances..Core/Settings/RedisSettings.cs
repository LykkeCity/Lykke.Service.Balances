using System;
using JetBrains.Annotations;

namespace Lykke.Service.Balances.Core.Settings
{
    [UsedImplicitly]
    public class RedisSettings
    {
        public string Configuration { get; set; }
        public string Instance { get; set; }
        public TimeSpan Expiration { get; set; }
    }
}
