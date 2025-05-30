using System.IdentityModel.Tokens.Jwt;
using DeathByAIBackend.Interfaces;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace DeathByAIBackend.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    private readonly string _secretKey =
        configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt Key configuration is missing.");
    private readonly string _issuer =
        configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt Issuer configuration is missing.");

    public string GenerateToken(string value)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Name, value)
            ]),
            Issuer = _issuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}