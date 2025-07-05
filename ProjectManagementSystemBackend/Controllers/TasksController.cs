using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models.DTO;
using System.Security.Claims;

namespace ProjectManagementSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        ApplicationContext _context;
        int _userId;
        ITaskHistory _taskHistoryService;
        public TasksController(ApplicationContext context, ITaskHistory taskHistoryService) 
        {
            _context = context;
            _taskHistoryService = taskHistoryService;
            _userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet]
        public async Task<IActionResult> Get(int baseBoadrId)
        {
            var availableBaseBoard = _context.BaseBoards
                .Include(bb => bb.Project)
                .ThenInclude(p => p.Participants)
                .AsNoTracking()
                .FirstOrDefaultAsync(bb => bb.Id == baseBoadrId &&
                    (bb.Project.Security == false || bb.Project.Participants.Any(p => p.UserId == _userId)));
            if (availableBaseBoard is null)
                return Unauthorized("You havent access to this action");

            var baseBoard = await _context.BaseBoards.Include(bb => bb.BoardStatuses).ThenInclude(bs => bs.Tasks).FirstOrDefaultAsync(bb => bb.Id == baseBoadrId);
            if(baseBoard is null) 
                return NotFound();
            var tasks = baseBoard.BoardStatuses.Select(bs => bs.Tasks);

            return tasks is null ? NotFound() : Ok(tasks);
        }
        [HttpPost]
        public async Task<IActionResult> Post(TaskDTO task)
        {
            var availableBoardStatus = await _context.BoardStatuses
                .Include(bs => bs.BaseBoard)
                .ThenInclude(bb => bb.Project)
                .ThenInclude(p => p.Participants)
                .AsNoTracking()
                .FirstOrDefaultAsync(bs => bs.Id == task.BoardStatusId &&
                (bs.BaseBoard.Project.Participants.Any(p => p.UserId == _userId && new int[]{ 1,2}.Any(r => r == p.RoleId))));
            if (availableBoardStatus is null)
                return Unauthorized();

            int currentParticipantId = availableBoardStatus.BaseBoard.Project.Participants
                .FirstOrDefault(p => p.UserId == _userId).Id;
            if (currentParticipantId is 0)
                return NotFound("Internal server error, participant not found");

            Models.Task newTask = new(task.Name,
                task.Description,
                task.Priority,
                DateTime.UtcNow,
                task.TimeLimit, 
                currentParticipantId, 
                task.ResponsiblePersonId,
                task.BoardStatusId);

            await _context.Tasks.AddAsync(newTask);
            await _context.SaveChangesAsync();

            await _taskHistoryService.CreateAsync(newTask,_userId)

            return Ok(newTask);
        }
        [HttpPut]
        public async Task<IActionResult> Update(TaskDTO updatedTask)
        {
            var availableTask = await _context.Tasks
                .Include(t => t.BoardStatus)
                .ThenInclude(bs => bs.BaseBoard)
                .ThenInclude(bb => bb.Project)
                .ThenInclude(p => p.Participants)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == updatedTask.Id && 
                    t.BoardStatus.BaseBoard.Project.Participants.Any(p => p.UserId == _userId && new int[] {1,2}.Any(r => r == p.RoleId)));
            if (availableTask is null)
                return Unauthorized();

            var task = await _context.Tasks.FindAsync(updatedTask.Id);
            if (task is null)
                return NotFound("Task not found");
            task.Name = updatedTask.Name;
            task.Description = updatedTask.Description;
            task.Priority = updatedTask.Priority;
            task.LastUpdate = DateTime.UtcNow;
            task.TimeLimit = updatedTask.TimeLimit;
            task.ResponsiblePersonId = updatedTask.ResponsiblePersonId;
            task.BoardStatusId = updatedTask.BoardStatusId;

            await _context.SaveChangesAsync();

            await _taskHistoryService.UpdateAsync(availableTask, task, _userId);

            return NoContent();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int taskId)
        {
            var availableTask = await _context.Tasks
                .Include(t => t.BoardStatus)
                .ThenInclude(t => t.BaseBoard)
                .ThenInclude(t => t.Project)
                .ThenInclude(t => t.Participants)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == taskId &&
                t.BoardStatus.BaseBoard.Project.Participants.Any(p => p.UserId == _userId && new int[] { 1, 2 }.Any(r => r == p.RoleId)));
            if (availableTask is null)
                return Unauthorized();

            var task = await _context.Tasks.FindAsync(taskId);
            if(task is null)
                return NotFound();

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            await _taskHistoryService.DeleteAsync(task, _userId);

            return NoContent();
        }

    }
}
