using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.Balances.Core.Services.Wallets;
using Lykke.Service.Registration.Models;

namespace Lykke.Job.Balances.RabbitSubscribers
{
    [UsedImplicitly]
    public class ClientAuthenticatedRabbitSubscriber : IStartable, IStopable
    {
        private readonly IWalletsManager _walletsManager;
        private readonly ILog _log;
        private readonly string _connectionString;
        private RabbitMqSubscriber<ClientAuthInfo> _subscriber;

        public ClientAuthenticatedRabbitSubscriber(IWalletsManager walletsManager, ILog log, string connectionString)
        {
            _walletsManager = walletsManager;
            _log = log;
            _connectionString = connectionString;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .CreateForSubscriber(_connectionString, "auth", "balances");

            _subscriber = new RabbitMqSubscriber<ClientAuthInfo>(settings, new DefaultErrorHandlingStrategy(_log, settings))
                .SetMessageDeserializer(new JsonMessageDeserializer<ClientAuthInfo>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .SetLogger(_log)
                .Start();
        }

        private async Task ProcessMessageAsync(ClientAuthInfo message)
        {
            var validationResult = ValidateMessage(message);
            if (validationResult.Any())
            {
                var error = $"Message will be skipped: {string.Join("\r\n", validationResult)}";
                await _log.WriteWarningAsync(nameof(BalanceUpdateRabbitSubscriber), nameof(ProcessMessageAsync), message.ToJson(), error);

                return;
            }

            await _walletsManager.CacheItAsync(message.ClientId);
        }

        private static IReadOnlyList<string> ValidateMessage(ClientAuthInfo message)
        {
            var errors = new List<string>();

            if (message == null)
            {
                errors.Add("message is null");
            }
            else
            {
                if (string.IsNullOrEmpty(message.ClientId))
                {
                    errors.Add("Empty client id");
                }
            }

            return errors;
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        public void Stop()
        {
            _subscriber?.Stop();
        }
    }
}