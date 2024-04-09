using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using System.Net;

namespace crafts_api.Problems
{
    public static class BuildInExceptionHandler
    {
        public static void AddErrorHandler(this IApplicationBuilder app, ILogger<CustomErrorMiddleware> logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();


                    if (contextFeature != null)
                    {
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                            {
                                StatusCode = context.Response.StatusCode,
                                Message = contextFeature.Error.Message
                            }));
                            logger.LogError(new EventId(10, "Internal Server Error"), contextFeature.Error, contextFeature.Error.Message);
                        }
                });
            });
        }
    }
}
