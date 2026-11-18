using Microsoft.AspNetCore.Diagnostics;

namespace Florarium.Api.Exceptions;

public sealed class ApiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ApiException apiException)
            return false;

        httpContext.Response.StatusCode = apiException.StatusCode;
        await httpContext.Response.WriteAsJsonAsync(new
        {
            code = apiException.Code,
            message = apiException.Message
        }, cancellationToken);

        return true;
    }
}
