using ClaimCommander.Models;

namespace ClaimCommander.Services
{
    public class InMemoryClaimStorageService : IClaimStorageService
    {
        private readonly Dictionary<int, Claim> _claims = new();
        private int _nextId = 1;
        private readonly object _lock = new();

        public int AddClaim(Claim claim)
        {
            lock (_lock)
            {
                claim.ClaimId = _nextId++;
                _claims[claim.ClaimId] = claim;
                return claim.ClaimId;
            }
        }

        public Claim? GetClaim(int claimId)
        {
            lock (_lock)
            {
                return _claims.TryGetValue(claimId, out var claim) ? claim : null;
            }
        }

        public List<Claim> GetAllClaims()
        {
            lock (_lock)
            {
                return _claims.Values.OrderByDescending(c => c.SubmissionDate).ToList();
            }
        }

        public List<Claim> GetClaimsByStatus(string status)
        {
            lock (_lock)
            {
                return _claims.Values
                    .Where(c => c.Status == status)
                    .OrderByDescending(c => c.SubmissionDate)
                    .ToList();
            }
        }

        public bool UpdateClaim(Claim claim)
        {
            lock (_lock)
            {
                if (_claims.ContainsKey(claim.ClaimId))
                {
                    _claims[claim.ClaimId] = claim;
                    return true;
                }
                return false;
            }
        }

        public bool DeleteClaim(int claimId)
        {
            lock (_lock)
            {
                return _claims.Remove(claimId);
            }
        }
    }
}