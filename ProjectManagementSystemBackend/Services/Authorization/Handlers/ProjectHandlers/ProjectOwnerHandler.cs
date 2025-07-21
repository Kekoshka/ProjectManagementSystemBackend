using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.Options;
using ProjectManagementSystemBackend.Services.Authorization.Requirements.ProjectRequirements;
using System.Security.Claims;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystemBackend.Services.Authorization.Handlers.ProjectHandlers
{
    public class ProjectOwnerHandler : AuthorizationHandler<ProjectOwnerRequirement, int>
    {
        ApplicationContext _context;
        int[] _ownerRoles;
        public ProjectOwnerHandler(ApplicationContext context, IOptions<RoleOptions> roleOptions)
        {
            _context = context;
            _ownerRoles = roleOptions.Value.OwnerRoles;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, 
            ProjectOwnerRequirement requirement, 
            int projectId)
        {
            int userId = Convert.ToInt32(context.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var isProjectAdmin = await _context.Projects
                .Include(p => p.Participants)
                .AsNoTracking()
                .AnyAsync(p => p.Id == projectId &&
                p.Participants.Any(p => p.UserId == userId &&
                    _ownerRoles.Any(ar => ar == p.RoleId)));

            if (isProjectAdmin)
                context.Succeed(requirement);
        }
    }
}
