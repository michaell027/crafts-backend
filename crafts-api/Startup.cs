using crafts_api.context;
using crafts_api.models;
using Microsoft.AspNetCore.Identity;

namespace crafts_api;

public static class Startup
{
    public static async Task InitializeDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<DatabaseContext>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        await DbInitializer.Seed(context, userManager);
    }
}