using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        ApplicationContext _context;
        IUserService _userService;
        public UsersController(ApplicationContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpPost("authorization")]
        public async Task<IActionResult> AuthorizationAsync(AuthData authData, CancellationToken cancellationToken)
        {
            try
            {
                string jwt = await _userService.AuthorizationAsync(authData, cancellationToken);
                return Ok(jwt);
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        
        }
        [HttpPost("registration")]
        public async Task<IActionResult> RegistrationAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                await _userService.RegistrationAsync(user, cancellationToken);
                return NoContent();
            }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
            }
    }
}
