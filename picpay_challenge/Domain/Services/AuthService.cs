
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace picpay_challenge.Domain.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly string? _secret;
        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
            _secret = configuration["Jwt:key"];

        }

        public string GenerateToken(string Email)
        {
            var claims = new[]
      {
            new Claim(JwtRegisteredClaimNames.Email, Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "putdomain.com",
                audience: "putdomain.com",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}