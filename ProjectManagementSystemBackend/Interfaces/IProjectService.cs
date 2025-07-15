using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystemBackend.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDTO>> GetAsync(int userId, CancellationToken cancellationToken);
        Task<ProjectDTO> CreateAsync(ProjectDTO project, int userId, CancellationToken cancellationToken);
        Task UpdateAsync(ProjectDTO project, CancellationToken cancellationToken);
        Task DeleteAsync(int projectId, CancellationToken cancellationToken);
    }
}
