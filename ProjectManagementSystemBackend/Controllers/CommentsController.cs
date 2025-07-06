using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using System.Security.Claims;

namespace ProjectManagementSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        ApplicationContext _context;
        int? userId;
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)); public CommentsController(ApplicationContext context) 
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

            var comments  = await _context.TaskComments.Where(c => c.TaskId == taskId).ToListAsync();
            return comments is null ? NotFound() : Ok(comments);
        }
        [HttpPost]
        public async Task<IActionResult> Post(CommentDTO comment)
        {
            var availableTask = await _context.Tasks
                .Include(t => t.BoardStatus)
                .ThenInclude(t => t.BaseBoard)
                .ThenInclude(t => t.Project)
                .ThenInclude(t => t.Participants)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == comment.TaskId && (
                    t.BoardStatus.BaseBoard.Project.Security == false ||
                    t.BoardStatus.BaseBoard.Project.Participants.Any(p => p.UserId == _userId)));
            if (availableTask is null)
                return Unauthorized("You havent access to this action");

            var participant = availableTask.BoardStatus.BaseBoard.Project.Participants.FirstOrDefault(p => p.UserId == _userId);
            if(participant is null)
                return NotFound("Participant not found");

            TaskComment newComment = new()
            {
                Message = comment.Message,
                ParticipantId = participant.Id,
                TaskId = comment.TaskId
            };
            await _context.TaskComments.AddAsync(newComment);
            await _context.SaveChangesAsync();

            CommentDTO commentDTO = new()
            {
                Id = newComment.Id,
                Message = newComment.Message,
                ParticipantId = newComment.ParticipantId,
                TaskId = newComment.TaskId
            };
            return Ok(commentDTO);
        }
        [HttpPut]
        public async Task<IActionResult> Update(CommentDTO comment)
        {
            var availableComment = await _context.TaskComments
                .Include(c => c.Participant)
                .FirstOrDefaultAsync(c => c.Id == comment.Id && c.Participant.UserId == _userId);
            if (availableComment is null)
                return Unauthorized("You havent access to this action");

            availableComment.Message = comment.Message;

            await _context.SaveChangesAsync();
            
            return NoContent();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int commentId)
        {
            var availableComment = await _context.TaskComments
                .Include(c => c.Participant)
                .FirstOrDefaultAsync(c => c.Id == commentId && c.Participant.UserId == _userId);
            if (availableComment is null)
                return Unauthorized("You havent access to this action");

            var comment = await _context.TaskComments.FindAsync(commentId);
            if(comment is null)
                return NotFound();

            _context.TaskComments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
