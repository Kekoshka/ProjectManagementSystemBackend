using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;

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
        public async Task<IActionResult> Get()
        {
            var roles = await _context.Roles.ToListAsync();
            if(roles is null)
                return NotFound();
            return Ok(roles);
        }
    }
}
