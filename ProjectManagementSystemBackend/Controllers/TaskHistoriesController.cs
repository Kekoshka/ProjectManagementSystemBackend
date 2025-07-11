﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using System.Security.Claims;
using IAuthorizationService = ProjectManagementSystemBackend.Interfaces.IAuthorizationService;

namespace ProjectManagementSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskHistoriesController : ControllerBase
    {
        ApplicationContext _context;
        IAuthorizationService _authorizationService;
        
        int? userId;
        int[] _userRoles = [1, 2, 3];
        int[] _adminRoles = [1, 2];
        int[] _ownerRoles = [1];
        int _userId => userId ??= Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

        public TaskHistoriesController(ApplicationContext context, IAuthorizationService authorizationService) 
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(int taskId, CancellationToken cancellationToken)
        {
            bool isAuthorize = await _authorizationService.AccessByTaskIdAsync(taskId, _userId, _userRoles, cancellationToken);
            if(!isAuthorize)
                return Unauthorized("You havent access to this action");

            var taskHistories = await _context.TaskHistories
                .Where(th => th.TaskId == taskId)
                .ToListAsync();
            return taskHistories is null ? NotFound() : Ok(taskHistories);
        }   
    }
}
