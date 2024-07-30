using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ReportBot.Common.Exceptions;
using System.Net;

namespace ReportBot.Middlewares;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.Result = context.Exception switch
        {
            NotFoundException => new NotFoundObjectResult(context.Exception.Message),
            InvalidCredentialsException => new UnauthorizedObjectResult(context.Exception.Message),
            IncorrectParametersException => new BadRequestObjectResult(context.Exception.Message),
            AlreadyExistsException => new BadRequestObjectResult(context.Exception.Message),
            UnauthorizedException => new UnauthorizedObjectResult(context.Exception.Message)
            {
                StatusCode = (int)HttpStatusCode.Forbidden
            },
            FluentValidation.ValidationException => new BadRequestObjectResult(context.Exception.Message),
            _ => new ObjectResult(new { error = $"An unexpected error occurred: {context.Exception.Message}" })
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            }
        };
    }
}
