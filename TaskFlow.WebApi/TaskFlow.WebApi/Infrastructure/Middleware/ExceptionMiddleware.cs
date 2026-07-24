using System.Net;
using TaskFlow.WebApi.Core.Exceptions;

namespace TaskFlow.WebApi.Infrastructure.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                await HandleExceptionAsync(context, e);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception e)
        {
            context.Response.ContentType = "application/json";

            var statusCode = e switch
            {
                KeyNotFoundException => HttpStatusCode.NotFound,            
                InvalidOperationException => HttpStatusCode.BadRequest,      
                UnauthorizedException => HttpStatusCode.Unauthorized,       
                ForbiddenException => HttpStatusCode.Forbidden,            
                UnauthorizedAccessException => HttpStatusCode.Forbidden,    
                _ => HttpStatusCode.InternalServerError                      
            };

            context.Response.StatusCode = (int)statusCode;

            object response;

            if (_env.IsDevelopment())
            {
                response = new
                {
                    statusCode = context.Response.StatusCode,
                    message = e.Message,
                    stackTrace = e.StackTrace?.ToString()
                };
            }
            else
            {  
                var message = statusCode == HttpStatusCode.InternalServerError
                    ? "Sunucu taraflı beklenmeyen bir hata oluştu."
                    : e.Message;

                response = new
                {
                    statusCode = context.Response.StatusCode,
                    message = message
                };
            }

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}