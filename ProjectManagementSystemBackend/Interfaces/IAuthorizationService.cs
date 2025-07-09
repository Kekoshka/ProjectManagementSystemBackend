namespace ProjectManagementSystemBackend.Interfaces
{
    public interface IAuthorizationService
    {
        Task<bool> AccessByProjectIdAsync(int projectId, int userId, int[] availableRolesId, CancellationToken cancellationToken);
        Task<bool> AccessByBoardIdAsync(int boardId, int userId, int[] availableRolesId, CancellationToken cancellationToken);
        Task<bool> AccessByBoardStatusIdAsync(int boardStatusId, int userId, int[] availableRolesId, CancellationToken cancellationToken);
        Task<bool> AccessByTaskIdAsync(int taskId, int userId, int[] availableRolesId, CancellationToken cancellationToken);
        Task<bool> AccessByCommentIdAsync(int commentId, int userId, CancellationToken cancellationToken);
        Task<bool> AccessByParticipantIdAsync(int participantId, int userId, int[] availableRolesId, CancellationToken cancellationToken);



    }
}
