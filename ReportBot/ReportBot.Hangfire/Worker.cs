using Hangfire.Abstractions;
using Hangfire.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ReportBot.Hangfire;

public class Worker : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public Worker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var hangfireService = scope.ServiceProvider.GetRequiredService<IHangfireService>();

        hangfireService.SetupRecurring<WorksnapsUserJob>(
            WorksnapsUserJob.Id,
            "*/10 * * * *");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
