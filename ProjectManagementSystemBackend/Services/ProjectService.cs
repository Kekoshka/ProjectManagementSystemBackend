using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystemBackend.Services
{
    public class ProjectService : IProjectService
    {
        ApplicationContext _context;
        IParticipantService _participantService;
        TypeAdapterConfig config = new TypeAdapterConfig();
        public ProjectService(ApplicationContext context, IParticipantService participantService) 
        {
            _context = context;
            _participantService = participantService;
        }
        public async Task<ProjectDTO> CreateAsync(ProjectDTO project, int userId, CancellationToken cancellationToken)
        {
            var newProject = project.Adapt<Project>(config.Fork(f => f.ForType<ProjectDTO,Project>().Ignore("Id")));
            await _context.Projects.AddAsync(newProject, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _participantService.CreateBaseParticipantAsync(newProject.Id, userId, cancellationToken);

            return newProject.Adapt<ProjectDTO>();
        }

        public async Task DeleteAsync(int projectId, CancellationToken cancellationToken)
        {
            var project = await _context.Projects.FindAsync(projectId,cancellationToken);
            if (project is null)
                throw new KeyNotFoundException();

            _context.Remove(project);
            await _context.SaveChangesAsync(cancellationToken);
        }

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

        public async Task UpdateAsync(ProjectDTO newProject, CancellationToken cancellationToken)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == newProject.Id, cancellationToken);
            if (project is null)
                throw new KeyNotFoundException();

            newProject.Adapt(project, config.Fork(f => f.ForType<Project, ProjectDTO>().Ignore("Id")));
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
