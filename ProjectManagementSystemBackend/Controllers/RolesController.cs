using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    {
        IRoleService _roleService;
        public RolesController(IRoleService roleService) 
        {
            _roleService = roleService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
        {
            var roles = _roleService.GetAsync(cancellationToken);

            return roles is null ? NotFound() : Ok(roles);
        }
    }
}
