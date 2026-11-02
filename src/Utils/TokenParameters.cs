using System.Text;

using giraf_core_v2.Utils;

using Microsoft.IdentityModel.Tokens;

public static class TokenParameters
{
    public static TokenValidationParameters Get()
    {
        return new TokenValidationParameters
        {
            // Validate signing key.
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Env.Get("JWT_SECRET"))),

            // Issuer and audience must match the ones specified in the environment variables.
            ValidateIssuer = true,
            ValidIssuer = Env.Get("JWT_ISSUER"),

            ValidateAudience = true,
            ValidAudiences = Env.Get("JWT_AUDIENCES")
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),

            // The JWT must not be expired.
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }
}