using System;
using Lykke.Job.Balances.Settings;
using Lykke.Sdk;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Job.Balances
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.ApiTitle = "Balances job API";
                options.Logs = ("BalancesJobLog", ctx => ctx.BalancesJob.Db.LogsConnString);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            app.UseLykkeConfiguration();
        }
    }
}
