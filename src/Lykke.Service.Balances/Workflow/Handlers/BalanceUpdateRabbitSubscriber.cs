using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Common;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Cqrs;
using Lykke.MatchingEngine.Connector.Models.Events;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Deduplication;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.Balances.Client;
using Lykke.Service.Balances.Client.Events;
using Lykke.Service.Balances.Core.Services;
using Lykke.Service.Balances.Settings;
using Lykke.Service.Balances.Workflow.Commands;

namespace Lykke.Service.Balances.Workflow.Handlers
{
    [UsedImplicitly]
    public class BalanceUpdateRabbitSubscriber : IStartable, IStopable
    {
        [NotNull] private readonly ILogFactory _logFactory;
        private readonly RabbitMqSettings _rabbitMqSettings;
        private readonly ICqrsEngine _cqrsEngine;
        private readonly List<IStopable> _subscribers = new List<IStopable>();

        private const string QueueName = "lykke.balances.updates";
        private const bool QueueDurable = true;

        public BalanceUpdateRabbitSubscriber(
            [NotNull] ICachedWalletsRepository cachedWalletsRepository,
            [NotNull] ILogFactory logFactory,
            [NotNull] RabbitMqSettings rabbitMqSettings,
            [NotNull] ICqrsEngine cqrsEngine)
        {
            _logFactory = logFactory;
            _rabbitMqSettings = rabbitMqSettings ?? throw new ArgumentNullException(nameof(rabbitMqSettings));
            _cqrsEngine = cqrsEngine ?? throw new ArgumentNullException(nameof(cqrsEngine));
        }

        public void Start()
        {
            _subscribers.Add(Subscribe<CashInEvent>(Lykke.MatchingEngine.Connector.Models.Events.Common.MessageType.CashIn, ProcessMessageAsync));
            _subscribers.Add(Subscribe<CashOutEvent>(Lykke.MatchingEngine.Connector.Models.Events.Common.MessageType.CashOut, ProcessMessageAsync));
            _subscribers.Add(Subscribe<CashTransferEvent>(Lykke.MatchingEngine.Connector.Models.Events.Common.MessageType.CashTransfer, ProcessMessageAsync));
            _subscribers.Add(Subscribe<ExecutionEvent>(Lykke.MatchingEngine.Connector.Models.Events.Common.MessageType.Order, ProcessMessageAsync));
        }

        private RabbitMqSubscriber<T> Subscribe<T>(Lykke.MatchingEngine.Connector.Models.Events.Common.MessageType messageType, Func<T, Task> func)
        {
            var settings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = _rabbitMqSettings.ConnectionString,
                QueueName = $"{QueueName}.{messageType}",
                ExchangeName = _rabbitMqSettings.Exchange,
                RoutingKey = ((int)messageType).ToString(),
                IsDurable = QueueDurable,
                DeadLetterExchangeName = $"{_rabbitMqSettings.Exchange}.dlx"
            };

            return new RabbitMqSubscriber<T>(
                    _logFactory,
                    settings,
                    new ResilientErrorHandlingStrategy(_logFactory, settings,
                        retryTimeout: TimeSpan.FromSeconds(10),
                        next: new DeadQueueErrorHandlingStrategy(_logFactory, settings)))
                .SetMessageDeserializer(new ProtobufMessageDeserializer<T>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(func)
                .CreateDefaultBinding()
                .SetAlternativeExchange(_rabbitMqSettings.AlternateConnectionString)
                .Start();
        }


        private Task ProcessMessageAsync(CashInEvent message)
        {
            UpdateBalances(message.Header, message.BalanceUpdates);
            UpdateTotalBalances(message.Header, message.BalanceUpdates);
            return Task.CompletedTask;
        }

        private Task ProcessMessageAsync(CashOutEvent message)
        {
            UpdateBalances(message.Header, message.BalanceUpdates);
            UpdateTotalBalances(message.Header, message.BalanceUpdates);
            return Task.CompletedTask;
        }

        private Task ProcessMessageAsync(CashTransferEvent message)
        {
            UpdateBalances(message.Header, message.BalanceUpdates);
            return Task.CompletedTask;
        }

        private Task ProcessMessageAsync(ExecutionEvent message)
        {
            if (message.BalanceUpdates == null)
                return Task.CompletedTask;

            UpdateBalances(message.Header, message.BalanceUpdates);
            return Task.CompletedTask;
        }

        private void UpdateBalances(Lykke.MatchingEngine.Connector.Models.Events.Common.Header header,
            List<Lykke.MatchingEngine.Connector.Models.Events.Common.BalanceUpdate> updates)
        {
            foreach (var wallet in updates)
            {
                _cqrsEngine.PublishEvent(new BalanceUpdatedEvent
                {
                    WalletId = wallet.WalletId,
                    AssetId = wallet.AssetId,
                    Balance = wallet.NewBalance,
                    Reserved = wallet.NewReserved,
                    OldBalance = wallet.OldBalance,
                    OldReserved = wallet.OldReserved,
                    SequenceNumber = header.SequenceNumber,
                    Timestamp = header.Timestamp
                }, BoundedContext.Name);
            }
        }

        private void UpdateTotalBalances(Lykke.MatchingEngine.Connector.Models.Events.Common.Header header,
            List<Lykke.MatchingEngine.Connector.Models.Events.Common.BalanceUpdate> updates)
        {
            foreach (var wallet in updates)
            {
                _cqrsEngine.SendCommand(new UpdateTotalBalanceCommand
                {
                    AssetId = wallet.AssetId,
                    BalanceDelta = ParseNullabe(wallet.NewBalance) - ParseNullabe(wallet.OldBalance),
                    SequenceNumber = header.SequenceNumber
                }, BoundedContext.Name, BoundedContext.Name);
            }
        }
        private decimal ParseNullabe(string value)
        {
            return !string.IsNullOrEmpty(value) ? decimal.Parse(value) : default;
        }

        public void Dispose()
        {
            Stop();
            foreach (var subscriber in _subscribers)
            {
                subscriber?.Dispose();
            }

        }

        public void Stop()
        {
            foreach (var subscriber in _subscribers)
            {
                subscriber?.Stop();
            }
        }
    }
}
