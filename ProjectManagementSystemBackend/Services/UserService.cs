using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystemBackend.Services
{
    public class UserService : IUserService
    {
        ApplicationContext _context; 
        IAuthenticationService _authenticationService; 
        IPasswordHasherService _passwordHasherService;
        public UserService(ApplicationContext context, IAuthenticationService authenticationService, IPasswordHasherService passwordHasherService) 
        {
            _context = context;
            _authenticationService = authenticationService;
            _passwordHasherService = passwordHasherService;
        }
        public async Task<string> AuthorizationAsync(AuthData authData, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == authData.Login, cancellationToken);
            if (user is null)
                throw new UnauthorizedAccessException("Invalid login or password");

            bool passwordIsValid = _passwordHasherService.Verify(authData.Password, user.Password);
            if (!passwordIsValid)
                throw new UnauthorizedAccessException("Invalid login or password");

            var jwt = _authenticationService.GetJWT(user);
            return jwt;
        }

        public async Task RegistrationAsync(User user, CancellationToken cancellationToken)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Login == user.Login, cancellationToken);
            if (existingUser is not null)
                throw new InvalidOperationException("user with such data is already exists");

            User newUser = new()
            {
                Login = user.Login,
                Name = user.Name,
                Password = _passwordHasherService.Hash(user.Password)
            };
            await _context.Users.AddAsync(newUser, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}
