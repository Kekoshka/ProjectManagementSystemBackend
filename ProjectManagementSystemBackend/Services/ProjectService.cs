using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models.DTO;

namespace ProjectManagementSystemBackend.Services
{
    public class ProjectService : IProjectService
    {
        public Task<ProjectDTO> CreateAsync(ProjectDTO project, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int projectId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProjectDTO>> GetAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(ProjectDTO project, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
