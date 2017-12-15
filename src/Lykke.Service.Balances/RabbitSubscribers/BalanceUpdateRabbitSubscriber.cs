using System;
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
using Lykke.Service.Balances.RabbitSubscribers.IncomingMessages;

namespace Lykke.Service.Balances.RabbitSubscribers
{
    [UsedImplicitly]
    public class BalanceUpdateRabbitSubscriber : IStartable, IStopable
    {
        private readonly IWalletsManager _walletsManager;
        private readonly ILog _log;
        private readonly string _connectionString;
        private RabbitMqSubscriber<BalanceUpdatedEventProjection> _subscriber;

        public BalanceUpdateRabbitSubscriber(IWalletsManager walletsManager, ILog log, string connectionString)
        {
            _walletsManager = walletsManager;
            _log = log;
            _connectionString = connectionString;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .CreateForSubscriber(_connectionString, "balanceupdate", "balances")
                .MakeDurable();

            _subscriber = new RabbitMqSubscriber<BalanceUpdatedEventProjection>(settings,
                    new ResilientErrorHandlingStrategy(_log, settings,
                        retryTimeout: TimeSpan.FromSeconds(10),
                        next: new DeadQueueErrorHandlingStrategy(_log, settings)))
                .SetMessageDeserializer(new JsonMessageDeserializer<BalanceUpdatedEventProjection>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .SetLogger(_log)
                .Start();
        }

        private async Task ProcessMessageAsync(BalanceUpdatedEventProjection message)
        {
            var validationResult = ValidateMessage(message);
            if (validationResult.Errors.Any())
            {
                var error = $"Message will be skipped: {string.Join("\r\n", validationResult.Errors)}";
                await _log.WriteWarningAsync(nameof(BalanceUpdateRabbitSubscriber), nameof(ProcessMessageAsync), message.ToJson(), error);

                return;
            }

            if (validationResult.Warnings.Any())
            {
                var warning = $"Message will be processed, but: {string.Join("\r\n", validationResult.Warnings)}";
                await _log.WriteWarningAsync(nameof(BalanceUpdateRabbitSubscriber), nameof(ProcessMessageAsync), message.ToJson(), warning);
            }

            // Processes clients in parallel, but assets within single client sequentially
            var tasks = message.Balances
                .Where(b => b.ClientId != null && b.Asset != null)
                .GroupBy(b => b.ClientId)
                .Select(g => Task.Run(async () =>
                    {
                        foreach (var b in g)
                        {
                            await _walletsManager.UpdateBalanceAsync(g.Key, b.Asset, b.NewBalance, b.NewReserved);
                        }
                    }
                ));

            await Task.WhenAll(tasks);
        }

        private static (IReadOnlyList<string> Warnings, IReadOnlyList<string> Errors) ValidateMessage(BalanceUpdatedEventProjection message)
        {
            var errors = new List<string>();

            if (message == null)
            {
                errors.Add("message is null");
            }
            else
            {
                if (message.Balances == null || !message.Balances.Any())
                {
                    errors.Add("Balances are empty");
                }
            }

            var warnings = new List<string>();

            if (message?.Balances != null)
            {
                for (var i = 0; i < message.Balances.Count; ++i)
                {
                    var balance = message.Balances[i];

                    if (balance.ClientId == null)
                    {
                        warnings.Add($"Balance {i} client id is empty");
                    }

                    if (balance.Asset == null)
                    {
                        warnings.Add($"Balance {i} asset is empty");
                    }
                }
            }

            return (warnings, errors);
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