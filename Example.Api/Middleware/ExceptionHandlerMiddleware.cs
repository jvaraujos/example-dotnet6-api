using Newtonsoft.Json;
using Example.Application.Contracts.Persistence.Interfaces.Logs;
using Example.Application.Exceptions;
using Example.Domain.Entities.Logs;
using System.Net;

namespace Example.Api.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _requestDelegate;

        public ExceptionHandlerMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task Invoke(HttpContext context, ILogErroRepository logErroRepository)
        {
            try
            {
                await _requestDelegate(context);
            }
            catch (Exception ex)
            {
                await logErroRepository.AddAsync(new LogErro
                {                    
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message,
                    InnerExceptionStackTrace = ex.InnerException?.StackTrace
                });
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            var result = string.Empty;
            switch (exception)
            {
                case ValidationException validationException:
                    httpStatusCode = HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(validationException.ValdationErrors);
                    break;
                case BadRequestException badRequestException:
                    httpStatusCode = HttpStatusCode.BadRequest;
                    result = badRequestException.Message;
                    break;
                case NotFoundException notFoundException:
                    httpStatusCode = HttpStatusCode.NotFound;
                    result = notFoundException.Message;
                    break;
                case IdentityException identityException:
                    httpStatusCode = HttpStatusCode.BadRequest;     
                    result = JsonConvert.SerializeObject(new { error = identityException.IdentityErrors });
                    break;
                case Exception:
                    httpStatusCode = HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(new { error = exception.Message });
                    break;
            }
            context.Response.StatusCode = (int)httpStatusCode;
            return context.Response.WriteAsync(result);
        }
    }
}
