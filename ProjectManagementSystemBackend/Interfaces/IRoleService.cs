using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDTO>> GetAsync(CancellationToken cancellationToken);
    }
}
