using System.Threading.Tasks;
using Lykke.Cqrs;
using Lykke.Sdk;

namespace Lykke.Service.Balances.Services
{
    public class StartupManager : IStartupManager
    {
        private readonly ICqrsEngine _engine;

        public StartupManager(ICqrsEngine engine)
        {
            _engine = engine;
        }

        public Task StartAsync()
        {
            _engine.StartSubscribers();

            return Task.CompletedTask;
        }
    }
}
