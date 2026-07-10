using System.Net;

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
        public async Task InvokeAsync(HttpContext context) {
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
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            object response;

            if(_env.IsDevelopment())
            {
                response = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = e.Message,
                    StackTrace = e.StackTrace?.ToString()
                };
            }
            else
            {
                response = new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "Internal Server Error"
                };
            }
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
