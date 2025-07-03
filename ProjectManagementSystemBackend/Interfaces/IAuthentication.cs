using ProjectManagementSystemBackend.Models;

namespace ProjectManagementSystemBackend.Interfaces
{
    public interface IAuthentication
    {
        string GetJWT(User user);
    }
}
