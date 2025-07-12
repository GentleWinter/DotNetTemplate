using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Template.Domain.Entities;

namespace Template.Application.Services;

public class TokenService
{
    public Task<string> GenerateToken(Client client)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("API_KEY") ?? throw new ArgumentException("Environment variable API_KEY is missing"));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, client.Name),
                new Claim(ClaimTypes.Role, client.Role.ToString()),
                new Claim(ClaimTypes.NameIdentifier, client.Id.ToString()),
            }),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        return Task.FromResult(tokenHandler.WriteToken(token));
    }
}