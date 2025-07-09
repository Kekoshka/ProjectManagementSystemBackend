using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Services
{
    public class RoleService : IRoleService
    {
        public Task<IEnumerable<RoleDTO>> GetAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
