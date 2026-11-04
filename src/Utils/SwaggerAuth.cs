using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;
namespace giraf_core_v2.Utils;

internal static class SwaggerAuth
{
    internal static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(o =>
        {
            o.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter your JWT token.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            };

            o.AddSecurityDefinition(
                JwtBearerDefaults.AuthenticationScheme,
                securityScheme);

            o.AddSecurityRequirement(document =>
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(
                        JwtBearerDefaults.AuthenticationScheme,
                        document)] = []
                });
        });

        return services;
    }
}