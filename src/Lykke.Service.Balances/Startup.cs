using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Lykke.Sdk;
using Lykke.Service.Balances.Settings;

namespace Lykke.Service.Balances
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.ApiTitle = "Balances API";
                options.Logs = ("BalancesServiceLog", ctx => ctx.BalancesService.Db.LogsConnString);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            app.UseLykkeConfiguration();
        }
    }
}
