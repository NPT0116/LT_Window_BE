using System;
using Microsoft.AspNetCore.Diagnostics;
using src.Exceptions;
using src.Utils;

namespace src.Middlewares;


    public class GlobalExceptionHandlers(ILogger<GlobalExceptionHandlers> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            Response<object> responseObj;

            if (exception is BaseException baseException)
            {
                responseObj = new Response<object>(
                    data: null,
                    message: baseException.Message,
                    success: false)
                {
                    Errors = new[] { baseException.Message }
                };

                httpContext.Response.StatusCode = (int)baseException.StatusCode;
                Console.WriteLine("Error: {0}", baseException.Message);
            }
            else
            {
                responseObj = new Response<object>(
                    data: null,
                    message: "An unexpected error occurred.",
                    success: false)
                {
                    Errors = new[] { "An unexpected error occurred." }
                };

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            logger.LogError("{Error}", responseObj.Message);

            await httpContext.Response.WriteAsJsonAsync(responseObj, cancellationToken)
                                 .ConfigureAwait(false);
            return true;
        }
    }