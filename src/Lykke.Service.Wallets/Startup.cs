using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common;
using Flurl.Http;
using Lykke.Service.Wallets.Core;
using Lykke.Service.Wallets.Modules;
using Lykke.SettingsReader;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.Swagger.Model;
using System;
using System.Collections.Generic;
using AzureStorage.Tables;
using Common.Log;
using Lykke.AzureQueueIntegration;
using Lykke.Logs;
using Lykke.Service.Wallets.Settings;
using Lykke.SlackNotification.AzureQueue;

namespace Lykke.Service.Wallets
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; }
        public IContainer ApplicationContainer { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            Environment = env;
        }

        public IConfigurationRoot Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var appSettings = Configuration.LoadSettings<AppSettings>();

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                });

            services.AddSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = "Wallets Service"
                });
                options.DescribeAllEnumsAsStrings();
            });

            services.AddLoggingWithSlack("LykkeWalletsServiceLog", appSettings.CurrentValue.SlackNotifications.AzureQueue, appSettings.ConnectionString(x => x.WalletsServiceSettings.Db.LogsConnString));

            var builder = new ContainerBuilder();
            builder.Populate(services);

            builder.RegisterModule(new ServiceModule(appSettings.Nested(s => s.WalletsServiceSettings)));            

            ApplicationContainer = builder.Build();
            return new AutofacServiceProvider(ApplicationContainer);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
           
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUi();

            appLifetime.ApplicationStopped.Register(() =>
            {
                var stoppableSerivces = ApplicationContainer.Resolve<IEnumerable<IStopable>>();

                foreach (var service in stoppableSerivces)
                    service.Stop();

                ApplicationContainer.Dispose();
            });
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static void AddLoggingWithSlack(this IServiceCollection services, string appName, AzureQueueSettings azureQueueSettings, IReloadingManager<string> dbLogConnectionStringManager)
        {
            var consoleLogger = new LogToConsole();
            var aggregateLogger = new AggregateLogger();

            aggregateLogger.AddLog(consoleLogger);

            // Creating slack notification service, which logs own azure queue processing messages to aggregate log
            var slackService = services.UseSlackNotificationsSenderViaAzureQueue(new AzureQueueIntegration.AzureQueueSettings
            {
                ConnectionString = azureQueueSettings.ConnectionString,
                QueueName = azureQueueSettings.QueueName
            }, aggregateLogger);

            var dbLogConnectionString = dbLogConnectionStringManager.CurrentValue;

            // Creating azure storage logger, which logs own messages to concole log
            if (!string.IsNullOrEmpty(dbLogConnectionString) && !(dbLogConnectionString.StartsWith("${") && dbLogConnectionString.EndsWith("}")))
            {
                var persistenceManager = new LykkeLogToAzureStoragePersistenceManager(
                    AzureTableStorage<LogEntity>.Create(dbLogConnectionStringManager, appName, consoleLogger),
                    consoleLogger);

                var slackNotificationsManager = new LykkeLogToAzureSlackNotificationsManager(slackService, consoleLogger);

                var azureStorageLogger = new LykkeLogToAzureStorage(
                    persistenceManager,
                    slackNotificationsManager,
                    consoleLogger);
                
                azureStorageLogger.Start();

                aggregateLogger.AddLog(azureStorageLogger);
            }
            
            services.AddSingleton<ILog>(aggregateLogger);
        }
    }
}
