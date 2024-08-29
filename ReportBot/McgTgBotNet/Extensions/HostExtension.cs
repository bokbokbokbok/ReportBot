using Hangfire.Abstractions;
using McgTgBotNet.Hangfire.Services;
using McgTgBotNet.Models;
using McgTgBotNet.Services;
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
using ReportBot.Hangfire;
using ReportBot.Services.Worksnaps;
using McgTgBotNet.MessageHandler;
using Telegram.Bot;
using McgTgBotNet.MessageHandler.Handlers;
using McgTgBotNet.Services.Interfaces;

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

    public static void AddMessageProcessor(this IServiceCollection services)
    {
        services.AddScoped<IMessageProcessor, MessageProcessor>();

        services.RegisterServicesFromAssembly<IMessageHandler>(ServiceLifetime.Scoped);

        services.AddSingleton<IHistoryContainer, HistoryContainer>();

        var client = new TelegramBotClient(ConfigExtension.GetConfiguration("TelegramBot:Token"));

        services.AddSingleton(client);
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddMessageProcessor();
        services.AddAutoMapper(typeof(ProjectProfile));
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

    private static void RegisterServicesFromAssembly<T>(this IServiceCollection services, ServiceLifetime lifetime)
    {
        var interfaceType = typeof(T);

        var implementations = interfaceType.Assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && interfaceType.IsAssignableFrom(t));

        foreach (var implementation in implementations)
        {
            services.Add(new ServiceDescriptor(interfaceType, implementation, lifetime));
        }
    }
}