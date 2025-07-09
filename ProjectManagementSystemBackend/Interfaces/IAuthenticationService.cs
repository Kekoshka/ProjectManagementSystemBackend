using ProjectManagementSystemBackend.Models;

namespace ProjectManagementSystemBackend.Interfaces
{
    public interface IAuthenticationService
    {
        string GetJWT(User user);
    }
}
