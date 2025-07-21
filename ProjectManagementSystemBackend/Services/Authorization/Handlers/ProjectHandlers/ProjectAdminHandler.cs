using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.Options;
using ProjectManagementSystemBackend.Services.Authorization.Requirements.ProjectRequirements;
using System.Security.Claims;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystemBackend.Services.Authorization.Handlers.ProjectHandlers
{
    public class ProjectAdminHandler : AuthorizationHandler<ProjectAdminRequirement, int>
    {
        ApplicationContext _context;
        int[] _adminRoles;
        public ProjectAdminHandler(ApplicationContext context, IOptions<RoleOptions> roleOptions)
        {
            _context = context;
            _adminRoles = roleOptions.Value.AdminRoles;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext authContext,
            ProjectAdminRequirement requirement,
            int projectId)
        {
            int userId = Convert.ToInt32(authContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var isProjectAdmin = await _context.Projects
                .Include(p => p.Participants)
                .AsNoTracking()
                .AnyAsync(p => p.Id == projectId &&
                p.Participants.Any(p => p.UserId == userId &&
                    _adminRoles.Any(ar => ar == p.RoleId)));

            if (isProjectAdmin)
                authContext.Succeed(requirement);
        }
    }
}
