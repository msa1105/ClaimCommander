using ClaimCommander.Models;
using System.Collections.Generic;
using System.Linq;

namespace ClaimCommander.Services
{
    public class InMemoryLecturerService : ILecturerService
    {
        private readonly List<LecturerInfo> _lecturers;

        public InMemoryLecturerService()
        {
            // Seed data: Matches the names in your ClaimStorageService seed data
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

        public void UpdateLecturer(LecturerInfo updatedLecturer)
        {
            var existing = _lecturers.FirstOrDefault(l => l.Id == updatedLecturer.Id);
            if (existing != null)
            {
                existing.Name = updatedLecturer.Name;
                existing.Email = updatedLecturer.Email;
                existing.HourlyRate = updatedLecturer.HourlyRate;
                existing.Department = updatedLecturer.Department;
            }
        }

        public decimal GetLecturerRate(int lecturerId)
        {
            var lecturer = _lecturers.FirstOrDefault(l => l.Id == lecturerId);
            return lecturer?.HourlyRate ?? 0m;
        }
    }
}