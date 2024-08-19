using Hangfire.Abstractions;
using McgTgBotNet.Hangfire.Services;
using McgTgBotNet.Models;
using McgTgBotNet.Services;
using McgTgBotNet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReportBot.DataBase.Repositories.Interfaces;
using ReportBot.DataBase.Repositories;
using ReportBot.Services.Services.Interfaces;
using ReportBot.Services.Services;
using McgTgBotNet.Profiles;
using Hangfire.Jobs;
using Microsoft.Extensions.Hosting;
using McgTgBotNet.Hangfire.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using ReportBot.Hangfire;
using ReportBot.Services.Worksnaps;

namespace McgTgBotNet.Extensions;

public static class HostExtension
{
    public static IHostBuilder CreateHostBuilder(this string[] args)
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());

        return Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            var configuration = hostContext.Configuration;
            services.AddHangfire(configuration);
            services.AddHostedService<Worker>();

            services.ConfigureServices();
            services.AddScoped<WorksnapsUserJob>();
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ProjectProfile));
        services.AddScoped<IMessageProcess, MessageProcess>();
        services.AddScoped<IWorksnapsService, WorksnapsService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IHangfireService, HangfireService>();
        services.AddScoped<IWorksnapsRepository, WorksnapsRepository>();
        services.AddHttpClient<IWorksnapsRepository, WorksnapsRepository>();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddDbContext<ApplicationDbContext>(opt =>
            opt.UseSqlServer(ConfigExtension.GetConfiguration("ConnectionStrings:DefaultConnection")));

    }
}