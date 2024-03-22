using crafts_api.interfaces;
using System.Net;

namespace crafts_api.Problems
{
    public class CustomErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomErrorMiddleware> logger;

        public CustomErrorMiddleware(RequestDelegate next, ILogger<CustomErrorMiddleware> logger)
        {
            _next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception error)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            if (error is IEventSchedulerException exception)
            {
                if (exception.StatusCode.HasValue)
                {
                    context.Response.StatusCode = (int)exception.StatusCode.Value;
                }
                await context.Response.WriteAsync(exception.ToJson());
                logger.LogError(exception.EventId, error, exception.Message);
            }
            else
            {
                await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "Unknown Error."
                }));
                logger.LogError(new EventId(10, "Unknown Error"), error, error.Message);
            }
            
        }
    }
}
