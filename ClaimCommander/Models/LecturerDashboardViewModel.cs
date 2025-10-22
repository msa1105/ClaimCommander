using System.Collections.Generic;

namespace ClaimCommander.Models
{
    public class LecturerDashboardViewModel
    {
        // Renamed from RecentClaims to avoid confusion, holds all claims for the view
        public List<Claim> AllClaims { get; set; } = new List<Claim>();

        // --- Summary Properties ---
        public decimal TotalHoursClaimed { get; set; }
        public decimal TotalAmountClaimed { get; set; }
        public int PendingClaimsCount { get; set; }

        // This can be used for the submission form if it's on the same page
        public NewClaimViewModel? NewClaimForm { get; set; }
    }
}