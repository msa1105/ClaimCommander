using ClaimCommander.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClaimCommander.Services
{
    /// <summary>
    /// In-memory user service to handle authentication and user retrieval.
    /// <para>
    /// Pre-seeded with users for: HR, Lecturer, Coordinator, and Manager.
    /// </para>
    /// </summary>
    public class InMemoryUserService : IUserService
    {
        private readonly List<User> _users;

        public InMemoryUserService()
        {
            _users = new List<User>
            {
                // 1. HR User
                new User
                {
                    UserId = 1,
                    FullName = "HR Administrator",
                    Email = "hr@test.com",
                    Password = "password",
                    Role = "HR",
                    Department = "Human Resources"
                },

                // 2. Lecturer User
                new User
                {
                    UserId = 2,
                    FullName = "John Doe",
                    Email = "lecturer@test.com",
                    Password = "password",
                    Role = "Lecturer",
                    Department = "Computer Science",
                    HourlyRate = 250.00m // HR Set Rate
                },

                // 3. Programme Coordinator
                new User
                {
                    UserId = 3,
                    FullName = "Sarah Connor",
                    Email = "coord@test.com",
                    Password = "password",
                    Role = "Coordinator",
                    Department = "Computer Science"
                },

                // 4. Academic Manager
                new User
                {
                    UserId = 4,
                    FullName = "Mike Ross",
                    Email = "manager@test.com",
                    Password = "password",
                    Role = "Manager",
                    Department = "Academic Affairs"
                }
            };
        }

        public User? Authenticate(string email, string password)
        {
            // In a real app, use password hashing!
            return _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && u.Password == password);
        }

        public User? GetUserById(int id)
        {
            return _users.FirstOrDefault(u => u.UserId == id);
        }
    }
}