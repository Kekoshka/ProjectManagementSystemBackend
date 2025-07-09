using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        ApplicationContext _context;
        public RolesController(ApplicationContext context) 
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
        {
            var roles = await _context.Roles
                .AsNoTracking()
                .ProjectToType<RoleDTO>()
                .ToListAsync(cancellationToken);
            return roles is null ? NotFound() : Ok(roles);
        }
    }
}
