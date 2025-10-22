using ClaimCommander.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClaimCommander.Services
{
    public class InMemoryClaimStorageService : IClaimStorageService
    {
        private readonly Dictionary<int, Claim> _claims = new();
        private int _nextId = 1;
        private readonly object _lock = new();

        public InMemoryClaimStorageService()
        {
            // Initialize with mock data by calling the existing AddClaim method
            AddClaim(new Claim
            {
                LecturerName = "John Doe",
                HoursWorked = 5,
                HourlyRate = 250.00m,
                SubmissionDate = DateTime.UtcNow.AddDays(-1),
                Status = "Pending",
                Notes = "Lecture preparation for midterm exams."
            });
            AddClaim(new Claim
            {
                LecturerName = "Jane Smith",
                HoursWorked = 8,
                HourlyRate = 275.00m,
                SubmissionDate = DateTime.UtcNow.AddDays(-2),
                Status = "Pending",
                Notes = "Grading of final year science projects."
            });
            AddClaim(new Claim
            {
                LecturerName = "Peter Jones",
                HoursWorked = 10,
                HourlyRate = 220.00m,
                SubmissionDate = DateTime.UtcNow.AddDays(-3),
                Status = "Pending",
                Notes = ""
            });
            AddClaim(new Claim
            {
                LecturerName = "John Doe",
                HoursWorked = 12,
                HourlyRate = 280.00m,
                SubmissionDate = DateTime.UtcNow.AddDays(-7),
                Status = "Approved",
                Notes = "Weekend workshop on digital art."
            });
            AddClaim(new Claim
            {
                LecturerName = "Mary Williams",
                HoursWorked = 7,
                HourlyRate = 210.00m,
                SubmissionDate = DateTime.UtcNow.AddDays(-8),
                Status = "Rejected",
                Notes = "Claim rejected. Hours submitted exceed timetable allocation."
            });
        }

        public int AddClaim(Claim claim)
        {
            lock (_lock)
            {
                claim.ClaimId = _nextId++;
                // Calculate total amount before storing
                claim.TotalAmount = (decimal)claim.HoursWorked * claim.HourlyRate;
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