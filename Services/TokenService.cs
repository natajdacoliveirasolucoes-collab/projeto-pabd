using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiFinanceiro.Models;
using Microsoft.IdentityModel.Tokens;

namespace ApiFinanceiro.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Generate(Usuario usuario)
        {
            var issuer = _configuration["Jwt:Issuer"] ?? "EventHub";
            var audience = _configuration["Jwt:Audience"] ?? "EventHub";
            var key = _configuration["Jwt:Key"] ?? "eventhub-dev-secret-key-change-me-32";
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
