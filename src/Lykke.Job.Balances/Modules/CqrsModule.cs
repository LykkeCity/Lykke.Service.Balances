using System.Collections.Generic;
using Autofac;
using Common.Log;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Cqrs.Configuration;
using Lykke.Job.Balances.RabbitSubscribers;
using Lykke.Job.Balances.Settings;
using Lykke.Messaging;
using Lykke.Messaging.RabbitMq;
using Lykke.SettingsReader;

namespace Lykke.Job.Balances.Modules
{
    public class CqrsModule : Module
    {
        private readonly CqrsSettings _settings;
        private readonly ILog _log;

        public CqrsModule(IReloadingManager<AppSettings> settingsManager, ILog log)
        {
            _settings = settingsManager.CurrentValue.BalancesJob.Cqrs;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            if (_settings.ChaosKitty != null)
            {
                builder
                    .RegisterType<ChaosKitty>()
                    .WithParameter(TypedParameter.From(_settings.ChaosKitty.StateOfChaos))
                    .As<IChaosKitty>()
                    .SingleInstance();
            }
            else
            {
                builder
                    .RegisterType<SilentChaosKitty>()
                    .As<IChaosKitty>()
                    .SingleInstance();
            }


            Messaging.Serialization.MessagePackSerializerFactory.Defaults.FormatterResolver = MessagePack.Resolvers.ContractlessStandardResolver.Instance;

            builder.Register(context => new AutofacDependencyResolver(context)).As<IDependencyResolver>().SingleInstance();

            var rabbitMqSettings = new RabbitMQ.Client.ConnectionFactory { Uri = _settings.RabbitConnectionString };

            var messagingEngine = new MessagingEngine(_log,
                new TransportResolver(new Dictionary<string, TransportInfo>
                {
                    {"RabbitMq", new TransportInfo(rabbitMqSettings.Endpoint.ToString(), rabbitMqSettings.UserName, rabbitMqSettings.Password, "None", "RabbitMq")}
                }),
                new RabbitMqTransportFactory());

            builder.RegisterType<BalancesUpdateProjection>();

            builder.Register(ctx =>
            {
                const string defaultRoute = "self";

                return new CqrsEngine(_log,
                    ctx.Resolve<IDependencyResolver>(),
                    messagingEngine,
                    new DefaultEndpointProvider(),
                    true,
                    Register.DefaultEndpointResolver(new RabbitMqConventionEndpointResolver(
                        "RabbitMq",
                        "protobuf",
                        environment: "lykke",
                        exclusiveQueuePostfix: "k8s")),

                    Register.BoundedContext("balances")
                        .PublishingEvents(typeof(BalanceUpdatedEvent))
                        .With(defaultRoute)
                        .ListeningEvents(typeof(BalanceUpdatedEvent))
                        .From("balances").On(defaultRoute)
                        .WithProjection(typeof(BalancesUpdateProjection), "balances")
                );
            })
            .As<ICqrsEngine>().SingleInstance().AutoActivate();
        }
    }
}
