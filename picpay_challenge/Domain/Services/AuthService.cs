
using Microsoft.IdentityModel.Tokens;
using picpay_challenge.Domain.Models.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static picpay_challenge.Domain.Models.User.BaseUser;
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

        public string GenerateToken(string Email, BaseUser.Roles Role, int Id)
        {
            var claims = new[]
      {
            new Claim(JwtRegisteredClaimNames.Email, Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, Role.ToString()),
            new Claim(ClaimTypes.NameIdentifier, Id.ToString())
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