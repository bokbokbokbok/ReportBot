using Hangfire.Abstractions;
using McgTgBotNet.Hangfire.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Hangfire;
using Hangfire.Jobs;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;

namespace McgTgBotNet.Hangfire.Extensions
{
    public static class HangfireExtensions
    {
        //public static void SetupHangfire(this IHost host)
        //{
        //    using var scope = host.Services.CreateScope();
        //    var services = scope.ServiceProvider;
        //    var hangfireService = services.GetRequiredService<IHangfireService>();

        //    hangfireService.SetupRecurring<WorksnapsUserJob>(
        //        WorksnapsUserJob.Id,
        //        "*/10 * * * *");
        //}

        public static void AddHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(
                cfg => cfg.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));

            JobStorage.Current = new SqlServerStorage(configuration.GetConnectionString("DefaultConnection"));

            services.AddHangfireServer();
            services.AddScoped<IHangfireService, HangfireService>();
        }
    }

}
