using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Common.CustomExceptions;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using ProjectManagementSystemBackend.Common.CustomExceptions;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystemBackend.Services
{
    /// <summary>
    /// Сервис для управления проектами
    /// </summary>
    /// <remarks>
    /// Позволяет получать, создавать, изменять и удалять проекты
    /// </remarks>
    public class ProjectService : IProjectService
    {
        ApplicationContext _context;
        IParticipantService _participantService;
        TypeAdapterConfig config = new TypeAdapterConfig();
        /// <summary>
        /// Конструктор сервиса для управления проектами
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        /// <param name="participantService">Сервис для управления участниками проекта</param>
        public ProjectService(ApplicationContext context, IParticipantService participantService) 
        {
            _context = context;
            _participantService = participantService;
        }
        /// <summary>
        /// Создать новый проект
        /// </summary>
        /// <param name="project">DTO с данными нового проекта</param>
        /// <param name="userId">ID пользователя</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>DTO с данными созданного проекта</returns>
        /// <remarks>
        /// Метод создает новый проект, а также добавляет нового участника
        /// и наделяет его ролью владельца проекта
        /// </remarks>
        public async Task<ProjectDTO> CreateAsync(ProjectDTO project, int userId, CancellationToken cancellationToken)
        {
            var newProject = project.Adapt<Project>(config.Fork(f => f.ForType<ProjectDTO,Project>().Ignore("Id")));
            await _context.Projects.AddAsync(newProject, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _participantService.CreateBaseParticipantAsync(newProject.Id, userId, cancellationToken);

            return newProject.Adapt<ProjectDTO>();
        }
        /// <summary>
        /// Удалить проект
        /// </summary>
        /// <param name="projectId">ID проекта</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <exception cref="NotFoundException">Если проект не найден</exception>
        public async Task DeleteAsync(int projectId, CancellationToken cancellationToken)
        {
            var project = await _context.Projects.FindAsync(projectId,cancellationToken);
            if (project is null)
                throw new NotFoundException($"Project with {projectId} id not found");

            _context.Remove(project);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Получить все проекты, доступные пользователю
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Список DTO проектов</returns>
        public async Task<IEnumerable<ProjectDTO>> GetAsync(int userId, CancellationToken cancellationToken)
        {
            var projects = await _context.Projects
                .Include(p => p.Participants)
                .Where(p => p.Security == false || p.Participants.Any(p => p.UserId == userId))
                .AsNoTracking()
                .ProjectToType<ProjectDTO>()
                .ToListAsync(cancellationToken);
            return projects;
        }

        /// <summary>
        /// Обновить данные проекта
        /// </summary>
        /// <param name="newProject">DTO с обновленными данными проекта</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <exception cref="NotFoundException">Если проект не найден</exception>
        public async Task UpdateAsync(ProjectDTO newProject, CancellationToken cancellationToken)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == newProject.Id, cancellationToken);
            if (project is null)
                throw new NotFoundException($"Project with {newProject.Id} id not found");

            newProject.Adapt(project, config.Fork(f => f.ForType<Project, ProjectDTO>().Ignore("Id")));
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
