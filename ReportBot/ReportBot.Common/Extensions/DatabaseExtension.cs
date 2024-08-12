using McgTgBotNet.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ReportBot.Common.Extensions;

public static class DatabaseExtension
{
    public static void MigrateDatabase(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }
}
