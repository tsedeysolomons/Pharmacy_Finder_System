using PharmacyFinder.API.Models.Entities;

namespace PharmacyFinder.API.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        int? ValidateToken(string token);
    }
}