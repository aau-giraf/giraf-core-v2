using System.Security.Claims;
using System.Text;

using giraf_core_v2.Utils;

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace giraf_core_v2.Auth;

public sealed class TokenProvider()
{
    public string Create(User user)
    {
        // Create signing credentials based on the JWT secret using HMAC-SHA256.
        // The signing credentials are used to validate the token on the receivers end.
        // It ensures that the token's payload has not been tampered with.
        string jwtSecret = Env.Get("JWT_SECRET");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        string audiencesString = Env.Get("JWT_AUDIENCES");
        string[] audiences = audiencesString.Split(",");

        // Provide details necessary to generate the JWT.
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.FirstName)
            ]),
            Expires = DateTime.UtcNow.AddMilliseconds(double.Parse(Env.Get("JWT_MAX_AGE_MS"))),
            SigningCredentials = credentials,
            Issuer = Env.Get("JWT_ISSUER"),
            Audiences = { }
        };

        // Add each audience member to the tokens audience list.
        // Audiences can not be assigned to directly, so we have to iterate and add one at a time to the empty list.
        foreach (string audience in audiences)
        {
            tokenDescriptor.Audiences.Add(audience);
        }

        // Create the JWT based on the descriptor.
        var handler = new JsonWebTokenHandler();
        string token = handler.CreateToken(tokenDescriptor);

        return token;
    }
}