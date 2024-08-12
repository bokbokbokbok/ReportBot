using Hangfire.Abstractions;
using McgTgBotNet.Hangfire.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Hangfire;
using Hangfire.Jobs;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using McgTgBotNet.Extensions;

namespace McgTgBotNet.Hangfire.Extensions
{
    public static class HangfireExtensions
    {
        public static void AddHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(
                cfg => cfg.UseSqlServerStorage(ConfigExtension.GetConfiguration("ConnectionStrings:DefaultConnection")));

            JobStorage.Current = new SqlServerStorage(ConfigExtension.GetConfiguration("ConnectionStrings:DefaultConnection"));

            services.AddHangfireServer();
            services.AddScoped<IHangfireService, HangfireService>();
        }
    }

}
