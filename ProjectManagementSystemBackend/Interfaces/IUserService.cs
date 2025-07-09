using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using ProjectManagementSystemBackend.Services;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystemBackend.Interfaces
{
    public interface IUserService
    {
        Task<string> AuthorizationAsync(AuthData authData, CancellationToken cancellationToken);
        Task RegistrationAsync(User user, CancellationToken cancellationToken);
    }
}
