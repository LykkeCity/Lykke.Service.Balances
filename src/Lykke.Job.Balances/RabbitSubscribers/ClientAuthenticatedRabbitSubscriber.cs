using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Job.Balances.Settings;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.Balances.Core.Services.Wallets;
using Lykke.Service.Registration.Models;

namespace Lykke.Job.Balances.RabbitSubscribers
{
    [UsedImplicitly]
    public class ClientAuthenticatedRabbitSubscriber : IStartable, IStopable
    {
        private readonly ICachedWalletsRepository _cachedWalletsRepository;
        private readonly ILog _log;
        private readonly ILogFactory _logFactory;
        private readonly RabbitMqSettings _rabbitMqSettings;
        private IStopable _subscriber;

        private const string QueueName = "balances";

        public ClientAuthenticatedRabbitSubscriber(
            [NotNull] ICachedWalletsRepository cachedWalletsRepository,
            [NotNull] ILogFactory logFactory,
            [NotNull] RabbitMqSettings rabbitMqSettings)
        {
            _cachedWalletsRepository = cachedWalletsRepository ?? throw new ArgumentNullException(nameof(cachedWalletsRepository));
            _logFactory = logFactory ?? throw new ArgumentNullException(nameof(logFactory));
            _rabbitMqSettings = rabbitMqSettings ?? throw new ArgumentNullException(nameof(rabbitMqSettings));
            _log = logFactory.CreateLog(this);
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .CreateForSubscriber(_rabbitMqSettings.ConnectionString, _rabbitMqSettings.Exchange, QueueName);

            _subscriber = new RabbitMqSubscriber<ClientAuthInfo>(
                    _logFactory,
                    settings, 
                    new DefaultErrorHandlingStrategy(_logFactory, settings))
                .SetMessageDeserializer(new JsonMessageDeserializer<ClientAuthInfo>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .Start();
        }

        private async Task ProcessMessageAsync(ClientAuthInfo message)
        {
            var validationResult = ValidateMessage(message);
            if (validationResult.Any())
            {
                var error = $"Message will be skipped: {string.Join("\r\n", validationResult)}";
                _log.Warning(error, context: message.ToJson());

                return;
            }

            await _cachedWalletsRepository.CacheItAsync(message.ClientId);
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
