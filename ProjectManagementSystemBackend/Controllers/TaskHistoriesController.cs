using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using System.Security.Claims;

namespace ProjectManagementSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskHistoriesController : ControllerBase
    {
        ApplicationContext _context;
        int? userId;
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

        public TaskHistoriesController(ApplicationContext context) 
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int taskId)
        {
            var availableTask = await _context.Tasks
                .Include(t => t.BoardStatus)
                .ThenInclude(t => t.BaseBoard)
                .ThenInclude(t => t.Project)
                .ThenInclude(t => t.Participants)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == taskId &&(
                    t.BoardStatus.BaseBoard.Project.Security == false ||
                    t.BoardStatus.BaseBoard.Project.Participants.Any(p => p.UserId == _userId)));
            if (availableTask is null)
                return Unauthorized("You havent access to this action");

            var taskHistories = await _context.TaskHistories
                .Where(th => th.TaskId == taskId)
                .ToListAsync();
            return taskHistories is null ? NotFound() : Ok(taskHistories);
        }   
    }
}
