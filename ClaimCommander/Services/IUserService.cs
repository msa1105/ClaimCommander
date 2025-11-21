using ClaimCommander.Models;

namespace ClaimCommander.Services
{
    public interface IUserService
    {
        User? Authenticate(string email, string password);
        User? GetUserById(int id);
    }
}