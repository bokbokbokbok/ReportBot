using Microsoft.AspNetCore.Http;
using ReportBot.Common.Exceptions;

namespace ReportBot.Common.Extensions;

public static class HttpContextExtension
{
    public static int GetUserId(this HttpContext context)
    {
        var claim = context.User.Claims.FirstOrDefault(c => c.Type == "worksnapsId");

        if (claim == null)
            throw new UnauthorizedException("Unauthorized");

        return int.Parse(claim.Value);
    }
}
