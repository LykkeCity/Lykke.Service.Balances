using Autofac;
using Lykke.Common.Chaos;
using Lykke.Common.Log;
using Lykke.Cqrs;
using Lykke.Cqrs.Configuration;
using Lykke.Messaging;
using Lykke.Messaging.Contract;
using Lykke.Messaging.RabbitMq;
using Lykke.Service.Balances.Settings;
using Lykke.SettingsReader;
using System.Collections.Generic;
using Lykke.Service.Balances.Workflow.Events;
using Lykke.Service.Balances.Workflow.Projections;

namespace Lykke.Service.Balances.Modules
{
    public class CqrsModule : Module
    {
        private readonly CqrsSettings _settings;

        public CqrsModule(IReloadingManager<AppSettings> settingsManager)
        {
            _settings = settingsManager.CurrentValue.BalancesService.Cqrs;
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

            builder.Register(ctx => new MessagingEngine(ctx.Resolve<ILogFactory>(),
                new TransportResolver(new Dictionary<string, TransportInfo>
                {
                    {
                        "RabbitMq",
                        new TransportInfo(rabbitMqSettings.Endpoint.ToString(), rabbitMqSettings.UserName,
                            rabbitMqSettings.Password, "None", "RabbitMq")
                    }
                }),
                new RabbitMqTransportFactory(ctx.Resolve<ILogFactory>()))).As<IMessagingEngine>().SingleInstance();

            builder.RegisterType<BalancesUpdateProjection>();

            builder.Register(ctx =>
            {
                const string defaultRoute = "self";

                return new CqrsEngine(ctx.Resolve<ILogFactory>(),
                    ctx.Resolve<IDependencyResolver>(),
                    ctx.Resolve<IMessagingEngine>(),
                    new DefaultEndpointProvider(),
                    true,
                    Register.DefaultEndpointResolver(new RabbitMqConventionEndpointResolver(
                        "RabbitMq",
                        Messaging.Serialization.SerializationFormat.ProtoBuf,
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
