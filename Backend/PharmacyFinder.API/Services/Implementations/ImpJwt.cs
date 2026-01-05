//using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
//using System.Threading.Tasks;

using System.Globalization;
//using PharmacyFinder.API.Models;
using PharmacyFinder.API.Models.Entities;


namespace PharmacyFinder.API.Services.Implementations
{
    public class JwtService(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

#pragma warning disable IDE0060 // Remove unused parameter
        public string GenerateToken(User user, object value)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            JwtSecurityTokenHandler tokenHandler = new();
#pragma warning disable CS8604 // Possible null reference argument.
            byte[] key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
#pragma warning restore CS8604 // Possible null reference argument.

#pragma warning disable CS8604 // Possible null reference argument.
            List<Claim> claims =
            [
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, "User") // You'll update this with actual roles
            ];
#pragma warning restore CS8604 // Possible null reference argument.

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

        internal string GenerateToken(User user)
        {
            throw new NotImplementedException();
        }
    }

}