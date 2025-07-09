using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        IPasswordHasherService _passwordHasherService;
        IAuthenticationService _authenticationService;
        public UsersController(ApplicationContext context,IPasswordHasherService passwordHasherService, IAuthenticationService authenticationService)
        {
            _context = context;
            _passwordHasherService = passwordHasherService;
            _authenticationService = authenticationService;
        }

        [HttpPost("authorization")]
        public async Task<IActionResult> AuthorizationAsync(AuthData authData)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == authData.Login);
            if (user is null)
                return Unauthorized("Invalid login or password");

            bool passwordIsValid  =_passwordHasherService.Verify(authData.Password, user.Password);
            if (!passwordIsValid)
                return Unauthorized("Invalid login or password");

            var jwt = _authenticationService.GetJWT(user);
            return Ok(jwt);
        }
        [HttpPost("registration")]
        public async Task<IActionResult> RegistrationAsync(User user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Login == user.Login);
            if (existingUser is not null)
                return Conflict("user with such data is already exists");

            User newUser = new()
            {
                Login = user.Login,
                Name = user.Name,
                Password = _passwordHasherService.Hash(user.Password)
            };
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
    }
}
