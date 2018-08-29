using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Sdk;
using Lykke.Service.Balances.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.Balances.Services
{
    public class ShutdownManager : IShutdownManager
    {
        private readonly ILog _log;
        private readonly List<IStopable> _items = new List<IStopable>();

        public ShutdownManager(ILogFactory logFactory, IEnumerable<IStartStop> stopables)
        {
            _log = logFactory.CreateLog(this);
            _items.AddRange(stopables);
        }

        public async Task StopAsync()
        {
            foreach (var item in _items)
            {
                try
                {
                    item.Stop();
                }
                catch (Exception ex)
                {
                    _log.Warning($"Unable to stop {item.GetType().Name}", ex);
                }
            }

            await Task.CompletedTask;
        }
    }
}
