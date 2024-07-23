using Hangfire.Abstractions;
using McgTgBotNet.Hangfire.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Hangfire;
using Hangfire.Jobs;
using McgTgBotNet.Services;
using ReportBot.Services.Services.Interfaces;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;

namespace McgTgBotNet.Hangfire.Extensions
{
    public static class HangfireExtensions
    {
        public static void SetupHangfire(this IHost host)
        {
            var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            var hangfireService = services.GetRequiredService<IHangfireService>();
            GlobalConfiguration.Configuration.UseSqlServerStorage("Server=(local);Database=ReportBot;Trusted_Connection=True;TrustServerCertificate=true;");

            hangfireService.SetupRecurring<WorksnapsUserJob>(
                WorksnapsUserJob.Id,
                "* * * * *");
        }

        public static void AddHangfire(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddHangfire(
                cfg => cfg.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));

            JobStorage.Current = new SqlServerStorage(configuration.GetConnectionString("DefaultConnection"));

            services.AddHangfireServer();
            services.AddScoped<IHangfireService, HangfireService>();
        }
    }

}
