using Hangfire;
using Microsoft.AspNetCore.Builder;

namespace McgTgBotNet;

public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHangfireDashboard("/hangfire");
        });
    }
}
