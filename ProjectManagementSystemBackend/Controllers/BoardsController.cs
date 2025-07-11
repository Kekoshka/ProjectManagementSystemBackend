using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using System.Security.Claims;

namespace ProjectManagementSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BoardsController : ControllerBase
    {
        IBoardService _boardService;
        Interfaces.IAuthorizationService _authorizationService;

        int? userId;
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
        int[] _userRoles = [1, 2, 3];
        int[] _adminRoles = [1, 2];
        int[] _ownerRoles = [1];

        public BoardsController(IBoardService boardService, Interfaces.IAuthorizationService authorizationService)
        {
            _boardService = boardService;
            _authorizationService = authorizationService;
        }
        [HttpGet("getBaseBoardsByProjectId")]
        public async Task<IActionResult> GetBoardsByProjectIdAsync(int projectId, CancellationToken cancellationToken)
        {
            var IsAuthorize = await _authorizationService.AccessByProjectIdAsync(projectId, _userId, _userRoles, cancellationToken);
            if (!IsAuthorize)
                return Unauthorized("You havent access to this action");

            var boards = _boardService.GetBoardsByProjectIdAsync(projectId, cancellationToken);

            return boards is null ? NotFound() : Ok(boards);
        }
        [HttpGet("getBoardByBaseBoardId")]
        public async Task<IActionResult> GetByBaseBoardIdAsync(int baseBoardId, CancellationToken cancellationToken)
        {
            bool IsAuthorize = await _authorizationService.AccessByBoardIdAsync(baseBoardId, _userId, _userRoles, cancellationToken);
            if (!IsAuthorize)
                return Unauthorized("You havent access to this action");

            object? abstractBoard = _boardService.GetByBaseBoardIdAsync(baseBoardId, cancellationToken);
            if (abstractBoard is CanbanBoardDTO cbDTO)
                return Ok(cbDTO);
            if (abstractBoard is ScrumBoardDTO sbDTO)
                return Ok(sbDTO);
            return NotFound();
            
        }
        [HttpPost("postCanbanBoard")]
        public async Task<IActionResult> PostCanbanAsync(CanbanBoardDTO canbanBoard, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByBoardIdAsync(canbanBoard.BaseBoard.Id, _userId, _adminRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            var newCanbanBoard = await _boardService.PostCanbanAsync(canbanBoard, cancellationToken);            
            return Ok(newCanbanBoard);
        }
        [HttpPost("postScrumBoard")]
        public async Task<IActionResult> PostScrumAsync(ScrumBoardDTO scrumBoard, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByProjectIdAsync(scrumBoard.BaseBoard.ProjectId, _userId, _adminRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            var newScrumBoard = await _boardService.PostScrumAsync(scrumBoard, cancellationToken);
            return Ok(newScrumBoard);
        }

        [HttpPut("updateCanbanBoard")]
        public async Task<IActionResult> UpdateCanbanBoardAsync(CanbanBoardDTO newCanbanBoard, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByBoardIdAsync(newCanbanBoard.BaseBoard.Id, _userId, _adminRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            try
            {
                await _boardService.UpdateCanbanBoardAsync(newCanbanBoard, cancellationToken);   
                return NoContent();
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch(InvalidOperationException ex) { return UnprocessableEntity(ex.Message); }
            catch(Exception) { return StatusCode(500, "InternalServerError"); }   
        }
        [HttpPut("updateScrumBoard")]
        public async Task<IActionResult> UpdateScrumBoardAsync(ScrumBoardDTO newScrumBoard, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByProjectIdAsync(newScrumBoard.BaseBoard.ProjectId, _userId, _adminRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            try
            {
                await _boardService.UpdateScrumBoardAsync(newScrumBoard, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return UnprocessableEntity(ex.Message); }
            catch (Exception) { return StatusCode(500, "InternalServerError"); }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int baseBoardId, CancellationToken cancellationToken)
        {
            bool isAuthorized = await _authorizationService.AccessByBoardIdAsync(baseBoardId, _userId, _ownerRoles, cancellationToken);
            if (!isAuthorized)
                return Unauthorized("You havent access to this action");

            try
            {
                await _boardService.DeleteAsync(baseBoardId, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (Exception) { return StatusCode(500, "InternalServerError")};
        }
    }
}
