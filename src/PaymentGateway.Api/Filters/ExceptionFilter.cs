using System;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using PaymentGateway.Domain.Exceptions;
using PaymentGateway.Models;

namespace PaymentGateway.Api.Filters
{
    public class ExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _accessor;

        public ExceptionFilter(ILoggerFactory logger, IHttpContextAccessor accessor)
        {
            _accessor = accessor;
            _logger = logger.CreateLogger<Exception>();
        }
        public Task OnExceptionAsync(ExceptionContext context)
        {
            context.Result = GetResult(_accessor.HttpContext, context);
            return Task.CompletedTask;
        }

        private IActionResult GetResult(HttpContext httpContext, ExceptionContext context)
        {
            var errorResponse = new ErrorResponse { TraceIdentifier = httpContext.TraceIdentifier};
            switch (context.Exception)
            {
                case ApiException apiException:
                {
                    _logger.LogError(apiException, "Api exception");
                    errorResponse = errorResponse with
                    {
                        ErrorCode = apiException.ErrorCode,
                        Message = apiException.ErrorMessage,
                        ErrorInformation = apiException.ErrorInformation
                    };
                    return new ContentResult
                    {
                        Content = JsonSerializer.Serialize(errorResponse),
                        ContentType = MediaTypeNames.Application.Json,
                        StatusCode = (int) apiException.StatusCode
                    };
                }
                case { } exception:
                {
                    _logger.LogError(exception, "Uncaught exception");
                    errorResponse = errorResponse with
                    {
                        ErrorCode = "UNCAUGHT_EXCEPTION",
                        Message = exception.Message
                    };
                    return new ContentResult
                    {
                        Content = JsonSerializer.Serialize(errorResponse),
                        ContentType = MediaTypeNames.Application.Json,
                        StatusCode = (int) HttpStatusCode.InternalServerError
                    };
                }
                default:
                    return new StatusCodeResult(500);
            }
        }
    }
}