using Hangfire.Abstractions;
using McgTgBotNet.Hangfire.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Hangfire;
using Hangfire.Jobs;
using Hangfire.SqlServer;

namespace McgTgBotNet.Hangfire.Extensions
{
    public static class HangfireExtensions
    {
        private static readonly WorksnapsUserJob _worksnapsUserJob = new WorksnapsUserJob();
        public static void SetupHangfire(this IHost host)
        {
            var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            var hangfireService = services.GetRequiredService<IHangfireService>();
            GlobalConfiguration.Configuration.UseSqlServerStorage("Server=(local);Database=ReportBotDb;Trusted_Connection=True;TrustServerCertificate=true;");

            hangfireService.SetupRecurring<WorksnapsUserJob>(
                _worksnapsUserJob.Id,
                "* * * * *");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
               Host.CreateDefaultBuilder(args)
                   .ConfigureServices((context, services) => services.AddSingleton<IHangfireService, HangfireService>());
    }

}
