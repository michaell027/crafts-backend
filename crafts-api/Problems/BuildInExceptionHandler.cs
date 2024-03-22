using crafts_api.interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using System.Net;

namespace crafts_api.Problems
{
    public static class BuildInExceptionHandler
    {
        public static void AddErrorHandler(this IApplicationBuilder app, ILogger<IEventSchedulerException> logger)
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
                        if (contextFeature.Error is IEventSchedulerException exception)
                        {
                            if (exception.StatusCode.HasValue)
                            {
                                context.Response.StatusCode = (int)exception.StatusCode.Value;
                            }
                            await context.Response.WriteAsync(exception.ToJson());
                            logger?.LogError(exception.EventId, contextFeature.Error, exception.Message);
                        }
                        else
                        {
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                            {
                                StatusCode = context.Response.StatusCode,
                                Message = "Internal Server Error."
                            }));
                            logger?.LogError(new EventId(10, "Unknown Error"), contextFeature.Error, contextFeature.Error.Message);
                        }
                    }
                });
            });
        }
    }
}
