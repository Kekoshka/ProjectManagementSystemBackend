using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using System.Security.Claims;
using IAuthorizationService = ProjectManagementSystemBackend.Interfaces.IAuthorizationService;

namespace ProjectManagementSystemBackend.Services
{
    /// <summary>
    /// Сервис авторизации и проверки прав доступа
    /// </summary>
    /// <remarks>
    /// Предоставляет методы для проверки прав пользователей на выполнение операций
    /// с различными сущностями системы (проектами, досками, задачами и т.д.)
    /// </remarks>
    public class AuthorizationService : IAuthorizationService
    {
        ApplicationContext _context;
        int _userRole = 3;
        int _adminRole = 2; 
        int _ownerRole = 1;

        /// <summary>
        /// Конструктор сервиса авторизации
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public AuthorizationService(ApplicationContext context) 
        {
            _context = context;
        }
        /// <summary>
        /// Проверить доступ к доске по ID
        /// </summary>
        /// <param name="boardId">ID доски</param>
        /// <param name="userId">ID пользователя</param>
        /// <param name="availableRolesId">Массив разрешенных ролей</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>
        /// True - если доступ разрешен, 
        /// False - если доступ запрещен
        /// </returns>
        /// <remarks>
        /// Логика проверки:
        /// 1. Для обычных пользователей: доступ разрешен, если проект публичный
        ///    или пользователь является участником проекта
        /// 2. Для администраторов/владельцев: доступ разрешен, если пользователь имеет
        ///    соответствующую роль в проекте
        /// </remarks>
        public async Task<bool> AccessByBoardIdAsync    (int boardId, int userId, int[] availableRolesId, CancellationToken cancellationToken)
        {
            var availableBoard = await _context.BaseBoards
                .Where(bb => availableRolesId.Any(ar => ar == _userRole)
                    ? bb.Project.Security == false 
                        || bb.Project.Participants.Any(p => p.UserId == userId)
                    : bb.Project.Participants.Any(p => p.UserId == userId && availableRolesId.Any(ar => ar == p.RoleId)))
                .Select(bb => new 
                {
                    Project = new
                    {
                        bb.Project.Security,
                        bb.Project.Participants
                    }
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            
            return availableBoard is null ? false : true;
        }

        /// <summary>
        /// Проверить доступ к статусу доски по ID
        /// </summary>
        /// <param name="boardStatusId">ID статуса доски</param>
        /// <param name="userId">ID пользователя</param>
        /// <param name="availableRolesId">Массив разрешенных ролей</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>
        /// True - если доступ разрешен, 
        /// False - если доступ запрещен
        /// </returns>
        /// <remarks>
        /// Логика проверки:
        /// 1. Для обычных пользователей: доступ разрешен, если проект публичный
        ///    или пользователь является участником проекта
        /// 2. Для администраторов/владельцев: доступ разрешен, если пользователь имеет
        ///    соответствующую роль в проекте        
        /// </remarks>
        public async Task<bool> AccessByBoardStatusIdAsync(int boardStatusId, int userId, int[] availableRolesId, CancellationToken cancellationToken)
        {

            var availableBoardStatus = await _context.BoardStatuses
                .Where(bs => availableRolesId.Any(ar => ar == _userRole)
                    ? bs.BaseBoard.Project.Security == false
                        || bs.BaseBoard.Project.Participants.Any(p => p.UserId == userId)
                    : bs.BaseBoard.Project.Participants.Any(p =>
                    p.UserId == userId && availableRolesId.Any(ar => ar == p.RoleId)))
                .Select(bs => new
                {
                    BaseBoard = new
                    {
                        Project = new
                        {
                            bs.BaseBoard.Project.Security,
                            bs.BaseBoard.Project.Participants
                        }
                    }
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            
                
            return availableBoardStatus is null ? false : true;
        }

        /// <summary>
        /// Проверить доступ к комментарию по ID
        /// </summary>
        /// <param name="commentId">ID комментария</param>
        /// <param name="userId">ID пользователя</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>
        /// True - если пользователь является автором комментария,
        /// False - в противном случае
        /// </returns>
        /// <remarks>
        /// Логика проверки:
        /// Доступ разрешен если пользователь является создателем комментария
        /// </remarks>
        public async Task<bool> AccessByCommentIdAsync(int commentId, int userId, CancellationToken cancellationToken)
        {
            var availableComment = await _context.TaskComments
                .Include(tc => tc.Participant)
                .FirstOrDefaultAsync(tc => tc.Participant.UserId == userId, cancellationToken);
            
            return availableComment is null ? false : true;
        }

        /// <summary>
        /// Проверить доступ к проекту по ID
        /// </summary>
        /// <param name="projectId">ID проекта</param>
        /// <param name="userId">ID пользователя</param>
        /// <param name="availableRolesId">Массив разрешенных ролей</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>
        /// True - если доступ разрешен,
        /// False - если доступ запрещен
        /// </returns>
        /// <remarks>
        /// Логика проверки:
        /// 1. Для обычных пользователей: доступ разрешен, если проект публичный
        ///    или пользователь является участником проекта
        /// 2. Для администраторов/владельцев: доступ разрешен, если пользователь имеет
        ///    соответствующую роль в проекте
        /// </remarks>
        public async Task<bool> AccessByProjectIdAsync(int projectId, int userId, int[] availableRolesId, CancellationToken cancellationToken)
        {
            var availableProject = await _context.Projects
                .Include(p => p.Participants)
                .AsNoTracking()
                .Where(p => availableRolesId.Any(ar => ar == _userRole)
                    ? p.Security == false
                        || p.Participants.Any(p => p.UserId == userId)
                    : p.Participants.Any(p => p.UserId == userId && availableRolesId.Any(ar => ar == p.RoleId)))
                .FirstOrDefaultAsync(cancellationToken);
            return availableProject is null ? false : true;
        }

        /// <summary>
        /// Проверить доступ к задаче по ID
        /// </summary>
        /// <param name="taskId">ID задачи</param>
        /// <param name="userId">ID пользователя</param>
        /// <param name="availableRolesId">Массив разрешенных ролей</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>
        /// True - если доступ разрешен,
        /// False - если доступ запрещен
        /// </returns>
        /// <remarks>
        /// Логика проверки через связь задачи с проектом:
        /// 1. Для обычных пользователей: доступ разрешен, если проект публичный
        ///    или пользователь является участником проекта
        /// 2. Для администраторов/владельцев: доступ разрешен, если пользователь имеет
        ///    соответствующую роль в проекте
        /// </remarks>
        public async Task<bool> AccessByTaskIdAsync(int taskId, int userId, int[] availableRolesId, CancellationToken cancellationToken)
        {
            var availableTask = await _context.Tasks
                .AsNoTracking()
                .Where(t => availableRolesId.Any(ar => ar == _userRole)
                    ? t.BoardStatus.BaseBoard.Project.Security == false
                        || t.BoardStatus.BaseBoard.Project.Participants.Any(p => p.UserId == userId)
                    : t.BoardStatus.BaseBoard.Project.Participants.Any(p => 
                        p.UserId == userId && availableRolesId.Any(ar => ar == p.RoleId)))
                .Select(t => new
                {
                    BoardStatus = new
                    {
                        BaseBoard = new
                        {
                            Project = new
                            {
                                t.BoardStatus.BaseBoard.Project.Security,
                                t.BoardStatus.BaseBoard.Project.Participants
                            }
                        }
                    }
                })
                .FirstOrDefaultAsync(cancellationToken);
            return availableTask is null ? false : true;
        }

        /// <summary>
        /// Проверить доступ к участнику проекта по ID
        /// </summary>
        /// <param name="participantId">ID участника проекта</param>
        /// <param name="userId">ID пользователя</param>
        /// <param name="availableRolesId">Массив разрешенных ролей</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>
        /// True - если доступ разрешен,
        /// False - если доступ запрещен
        /// </returns>
        /// <remarks>
        /// Логика проверки через связь участника с проектом:
        /// 1. Для обычных пользователей: доступ разрешен, если проект публичный
        ///    или пользователь является участником проекта
        /// 2. Для администраторов/владельцев: доступ разрешен, если пользователь имеет
        ///    соответствующую роль в проекте
        /// </remarks>
        public async Task<bool> AccessByParticipantIdAsync(int participantId, int userId, int[] availableRolesId, CancellationToken cancellationToken)
        {
            var availableParticipant = await _context.Participants
                .Include(p => p.Project)
                .ThenInclude(p => p.Participants)
                .Where(p => availableRolesId.Any(ar => ar == _userRole)
                ? p.Project.Security == false || p.Project.Participants.Any(p => p.UserId == userId)
                : p.Project.Participants.Any(p =>
                        p.UserId == userId && availableRolesId.Any(ar => ar == p.RoleId)))
                .FirstOrDefaultAsync(cancellationToken);
            return availableParticipant is null ? false : true;
        }
    }
}
