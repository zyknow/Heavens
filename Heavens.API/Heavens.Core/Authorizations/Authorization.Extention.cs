using Microsoft.Extensions.DependencyInjection;

namespace Heavens.Core.Authorizations;

public static class Authorization
{
    public static void AddAuth(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Users",
                 policy => policy.RequireRole("admin", "user"));
        });

    }
}
