using System.Globalization;
//using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
//using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PharmacyFinder.API.Models.Entities;
using PharmacyFinder.API.Services.Interfaces;

namespace PharmacyFinder.API.Services.Implementations
{
    public class JwtService(IConfiguration configuration) : IJwtService
    {
        private readonly IConfiguration _configuration = configuration;

        public string GenerateToken(User user)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            List<Claim> claims =
            [
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
            ];

            claims.AddRange(user.UserRoles.Select(ur => new Claim(ClaimTypes.Role, ur.Role.RoleName)));

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(
                    Convert.ToDouble(_configuration["Jwt:ExpiryInHours"], CultureInfo.InvariantCulture)),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public int? ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            try
            {
                _ = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                JwtSecurityToken jwtToken = (JwtSecurityToken)validatedToken;
                int userId = int.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value, CultureInfo.InvariantCulture);

                return userId;
            }
            catch
            {
                return null;
            }
        }
    }
}