using Hangfire.Abstractions;
using McgTgBotNet.Hangfire.Services;
using McgTgBotNet.Models;
using McgTgBotNet.Services;
using McgTgBotNet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportBot.DataBase.Repositories.Interfaces;
using ReportBot.DataBase.Repositories;
using ReportBot.Services.Services.Interfaces;
using ReportBot.Services.Services;
using McgTgBotNet.Profiles;
using McgTgBotNet.Hangfire.Extensions;
using Hangfire;

namespace McgTgBotNet.Extensions;

public static class HostExtension
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ProjectProfile));
        services.AddScoped<IMessageProcess, MessageProcess>();
        services.AddScoped<IWorksnapsService, WorksnapsService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IHangfireService, HangfireService>();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddDbContext<ApplicationDbContext>(opt =>
            opt.UseSqlServer("Server=(local);Database=ReportBot;Trusted_Connection=True;TrustServerCertificate=true;"));

    }
}
