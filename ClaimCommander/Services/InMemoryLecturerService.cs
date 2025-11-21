using ClaimCommander.Models;
using System.Collections.Generic;
using System.Linq;

namespace ClaimCommander.Services
{
    /// <summary>
    /// Service to manage lecturer details including their specific hourly rates.
    /// <para>
    /// References:
    /// <list type="bullet">
    /// <item>Microsoft (2025) 'Dependency injection in ASP.NET Core', available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection</item>
    /// </list>
    /// </para>
    /// </summary>
    public class InMemoryLecturerService : ILecturerService
    {
        private readonly List<LecturerInfo> _lecturers;

        public InMemoryLecturerService()
        {
            // Seed data: Matches the names in your ClaimStorageService seed data
            // Requirement: HR sets these rates. We seed them here to simulate that state.
            _lecturers = new List<LecturerInfo>
            {
                new LecturerInfo { Id = 1, Name = "John Doe", Email = "john.doe@example.com", Department = "Computer Science", HourlyRate = 250.00m },
                new LecturerInfo { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com", Department = "Physics", HourlyRate = 275.00m },
                new LecturerInfo { Id = 3, Name = "Peter Jones", Email = "peter.jones@example.com", Department = "Engineering", HourlyRate = 220.00m },
                new LecturerInfo { Id = 4, Name = "Mary Williams", Email = "mary.williams@example.com", Department = "Mathematics", HourlyRate = 210.00m }
            };
        }

        public List<LecturerInfo> GetAllLecturers()
        {
            return _lecturers;
        }

        public LecturerInfo? GetLecturer(int id)
        {
            return _lecturers.FirstOrDefault(l => l.Id == id);
        }

        /// <summary>
        /// Updates lecturer details. Used by HR to maintain accounts.
        /// </summary>
        public void UpdateLecturer(LecturerInfo updatedLecturer)
        {
            var existing = _lecturers.FirstOrDefault(l => l.Id == updatedLecturer.Id);
            if (existing != null)
            {
                existing.Name = updatedLecturer.Name;
                existing.Email = updatedLecturer.Email;
                // Automation Rule: This sets the rate used for calculations.
                existing.HourlyRate = updatedLecturer.HourlyRate;
                existing.Department = updatedLecturer.Department;
            }
        }

        /// <summary>
        /// Retrieves the official rate set by HR. 
        /// Used for automation to prevent lecturers from setting their own rates.
        /// </summary>
        public decimal GetLecturerRate(int lecturerId)
        {
            var lecturer = _lecturers.FirstOrDefault(l => l.Id == lecturerId);
            return lecturer?.HourlyRate ?? 0m;
        }
    }
}