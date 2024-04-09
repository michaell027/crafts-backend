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
                await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    StatusCode = context.Response.StatusCode,
                    Message = context.Response.StatusCode == 500 ? "Internal Server Error" : error.Message
                }));
                logger.LogError(new EventId(10, "Unknown Error"), error, error.Message);
            
        }
    }
}
