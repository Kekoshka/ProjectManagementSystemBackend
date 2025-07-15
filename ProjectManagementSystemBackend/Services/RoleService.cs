using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Services
{
    public class RoleService : IRoleService
    {
        ApplicationContext _context;
        public RoleService(ApplicationContext context) 
        {
            _context = context;
        }
        public async Task<IEnumerable<RoleDTO>> GetAsync(CancellationToken cancellationToken)
        {
            var roles = await _context.Roles
                .AsNoTracking()
                .ProjectToType<RoleDTO>()
                .ToListAsync(cancellationToken);
            return roles;
        }
    }
}
