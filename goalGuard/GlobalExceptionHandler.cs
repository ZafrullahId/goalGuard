using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace goalGuard
{
    public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError("Exception occurred: {Message} \n {InnerMessage}", exception.Message, exception.InnerException?.Message);
            var problemDetails = CreateProblemDetailFromException(exception);
            problemDetails.Instance = httpContext.Request.Path;

            httpContext.Response.StatusCode = problemDetails.Status!.Value;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);
            return true;
        }

        private static ProblemDetails CreateProblemDetailFromException(Exception exception)
        {
            return exception is Refit.ApiException e
                ? new ProblemDetails
                {
                    Status = (int)e.StatusCode,
                    Title = "Bad Request",
                    Detail = e.Message,
                }
                : new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Server error",
                    Detail = "An error occured",

                };
        }
    }
}