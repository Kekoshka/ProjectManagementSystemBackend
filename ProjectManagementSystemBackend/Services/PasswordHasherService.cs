using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using ProjectManagementSystemBackend.Interfaces;

namespace ProjectManagementSystemBackend.Services
{
    public class PasswordHasherService : IPasswordHasher
    {
        private readonly int _workFactor = 14;
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, _workFactor);
        }

        public bool Verify(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
