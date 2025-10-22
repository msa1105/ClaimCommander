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
        }
    }
}