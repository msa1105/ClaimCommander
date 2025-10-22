using ClaimCommander.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ClaimCommander.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any users. If there are any, the DB has been seeded.
            if (context.Users.Any())
            {
                return;
            }

            var users = new User[]
            {
                new User{FullName="Alex Archer (Lecturer)", Email="lecturer@example.com", Password="password123", Department="Computer Science", Role="Lecturer", HourlyRate=350.00m},
                new User{FullName="Sam Smith (Admin)", Email="admin@example.com", Password="password123", Department="Administration", Role="Admin", HourlyRate=0}
            };
            context.Users.AddRange(users);
            context.SaveChanges();

            var subjects = new Subject[]
            {
                new Subject{Name="Advanced Programming"},
                new Subject{Name="Database Systems"}
            };
            context.Subjects.AddRange(subjects);
            context.SaveChanges();

            // --- ADD SAMPLE CLAIMS ---

            // Get the ID of the lecturer and subjects we just created
            var lecturerId = context.Users.Single(u => u.Email == "lecturer@example.com").UserId;
            var subject1Id = context.Subjects.Single(s => s.Name == "Advanced Programming").SubjectId;
            var subject2Id = context.Subjects.Single(s => s.Name == "Database Systems").SubjectId;

            var claims = new Claim[]
            {
                new Claim
                {
                    LecturerId = lecturerId,
                    SubjectId = subject1Id,
                    HoursWorked = 25.5m,
                    ClaimValue = 25.5m * 350.00m,
                    SubmissionDate = DateTime.Now.AddDays(-10),
                    Status = "Approved"
                },
                new Claim
                {
                    LecturerId = lecturerId,
                    SubjectId = subject2Id,
                    HoursWorked = 18m,
                    ClaimValue = 18m * 350.00m,
                    SubmissionDate = DateTime.Now.AddDays(-5),
                    Status = "Pending"
                },
                new Claim
                {
                    LecturerId = lecturerId,
                    SubjectId = subject1Id,
                    HoursWorked = 15m,
                    ClaimValue = 15m * 350.00m,
                    SubmissionDate = DateTime.Now.AddMonths(-1),
                    Status = "Rejected"
                }
            };
            context.Claims.AddRange(claims);
            context.SaveChanges();
        }
    }
}