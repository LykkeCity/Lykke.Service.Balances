using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Job.Balances.Settings;
using Lykke.MatchingEngine.Connector.Models.Events;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.Balances.Core.Services.Wallets;

namespace Lykke.Job.Balances.RabbitSubscribers
{
    [UsedImplicitly]
    public class BalancesUpdateConsumer : IStartable, IStopable
    {
        private readonly ICachedWalletsRepository _cachedWalletsRepository;
        private readonly ILog _log;
        private readonly RabbitMqSettings _rabbitMqSettings;
        private readonly List<IStopable> _subscribers = new List<IStopable>();
        
        private const string QueueName = "lykke.balances.updates";
        private const bool QueueDurable = true;

        public BalancesUpdateConsumer(ICachedWalletsRepository cachedWalletsRepository, ILog log, RabbitMqSettings rabbitMqSettings)
        {
            _cachedWalletsRepository = cachedWalletsRepository;
            _log = log;
            _rabbitMqSettings = rabbitMqSettings;
        }

        public void Start()
        {
            _subscribers.Add(Subscribe<CashInEvent>(MatchingEngine.Connector.Models.Events.Common.MessageType.CashIn, ProcessMessageAsync));
            _subscribers.Add(Subscribe<CashOutEvent>(MatchingEngine.Connector.Models.Events.Common.MessageType.CashOut, ProcessMessageAsync));
            _subscribers.Add(Subscribe<CashTransferEvent>(MatchingEngine.Connector.Models.Events.Common.MessageType.CashTransfer, ProcessMessageAsync));
            _subscribers.Add(Subscribe<ExecutionEvent>(MatchingEngine.Connector.Models.Events.Common.MessageType.Order, ProcessMessageAsync));
        }

        private RabbitMqSubscriber<T> Subscribe<T>(MatchingEngine.Connector.Models.Events.Common.MessageType messageType, Func<T, Task> func)
        {
            var settings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = _rabbitMqSettings.ConnectionString,
                QueueName = $"{QueueName}.{messageType}",
                ExchangeName = _rabbitMqSettings.Exchange,
                RoutingKey = ((int)messageType).ToString(),
                IsDurable = QueueDurable
            };

            return new RabbitMqSubscriber<T>(settings,
                    new ResilientErrorHandlingStrategy(_log, settings,
                        retryTimeout: TimeSpan.FromSeconds(10),
                        next: new DeadQueueErrorHandlingStrategy(_log, settings)))
                .SetMessageDeserializer(new ProtoSerializer<T>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(func)
                .CreateDefaultBinding()
                .SetLogger(_log)
                .Start();
        }
        

        private Task ProcessMessageAsync(CashInEvent message)
        {
            return UpdateBalances(message.Header, message.BalanceUpdates);
        }

        private Task ProcessMessageAsync(CashOutEvent message)
        {
            return UpdateBalances(message.Header, message.BalanceUpdates);
        }

        private Task ProcessMessageAsync(CashTransferEvent message)
        {
            return UpdateBalances(message.Header, message.BalanceUpdates);
        }

        private Task ProcessMessageAsync(ExecutionEvent message)
        {
            if(message.BalanceUpdates == null)
                return Task.CompletedTask;

            return UpdateBalances(message.Header, message.BalanceUpdates);
        }

        private async Task UpdateBalances(MatchingEngine.Connector.Models.Events.Common.Header header,
            List<MatchingEngine.Connector.Models.Events.Common.BalanceUpdate> updates)
        {
            if (updates.Count == 0)
                return;

            foreach (var balance in updates)
            {
                // todo: use cqrs command to update balances
                await _cachedWalletsRepository.UpdateBalanceAsync(
                    balance.WalletId,
                    balance.AssetId,
                    decimal.Parse(balance.NewBalance),
                    decimal.Parse(balance.NewReserved),
                    header.SequenceNumber);
            }
        }

        public void Dispose()
        {
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
