using ClaimCommander.Models;

namespace ClaimCommander.Services
{
    public interface IClaimStorageService
    {
        int AddClaim(Claim claim);
        Claim? GetClaim(int claimId);
        List<Claim> GetAllClaims();
        List<Claim> GetClaimsByStatus(string status);
        bool UpdateClaim(Claim claim);
        bool DeleteClaim(int claimId);
    }
}