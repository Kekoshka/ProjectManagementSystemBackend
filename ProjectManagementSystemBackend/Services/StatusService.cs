using Mapster;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystemBackend.Services
{
    /// <summary>
    /// Сервис для управления статусами досок
    /// </summary>
    /// <remarks>
    /// Позволяет получать, создавать, изменять и удалять статусы досок
    /// </remarks>
    public class StatusService : IStatusService
    {
        ApplicationContext _context;
        TypeAdapterConfig config = new TypeAdapterConfig();
        
        /// <summary>
        /// Конструктор сервиса статусов
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public StatusService(ApplicationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Удалить статус доски
        /// </summary>
        /// <param name="boardStatusId">ID статуса доски</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <exception cref="KeyNotFoundException">Если статус доски не найден </exception>
        public async Task DeleteAsync(int boardStatusId, CancellationToken cancellationToken)
        {
            var status = await _context.BoardStatuses.FindAsync(boardStatusId, cancellationToken);
            if (status is null)
                throw new KeyNotFoundException();
            _context.BoardStatuses.Remove(status);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Получить статусы доски
        /// </summary>
        /// <param name="baseBoardId">ID базовой доски</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Список DTO статусов доски</returns>
        public async Task<IEnumerable<StatusDTO>> GetAsync(int baseBoardId, CancellationToken cancellationToken)
        {
            var statuses = await _context.BoardStatuses
                            .Include(bs => bs.Status)
                            .ProjectToType<StatusDTO>()
                            .Where(bs => bs.BaseBoardId == baseBoardId)
                            .ToListAsync(cancellationToken);
            return statuses;
        }
        /// <summary>
        /// Опубликовать новый статус доски
        /// </summary>
        /// <param name="status">DTO с данными нового статуса доски</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>DTO с данными созданого статуса доски</returns>
        /// <remarks>
        /// Проверяет существует ли статус с переданным названием,
        /// если статуса с таким названием нет то создается новый статус, после чего создается статус доски
        /// </remarks>
        /// <exception cref="KeyNotFoundException">Если базовая доска не была найдена</exception>
        public async Task<StatusDTO> PostAsync(StatusDTO status, CancellationToken cancellationToken)
        {
            var existsStatus = await _context.Statuses.FirstOrDefaultAsync(s => s.Name == status.Name, cancellationToken);
            if (existsStatus is null)
            {
                Status newStatus = new() { Name = status.Name };
                await _context.Statuses.AddAsync(newStatus, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                existsStatus = newStatus;
            }
            var existsBaseBoard = await _context.BaseBoards.FindAsync(status.BaseBoardId, cancellationToken);
            if (existsBaseBoard is null)
                throw new KeyNotFoundException($"BaseBoard with {status.BaseBoardId} id not found");
            BoardStatus newBoardStatus = new()
            {
                BaseBoardId = status.BaseBoardId,
                StatusId = existsStatus.Id,
            };
            await _context.BoardStatuses.AddAsync(newBoardStatus, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return newBoardStatus.Adapt<StatusDTO>();
        }

        /// <summary>
        /// Обновить статус доски
        /// </summary>
        /// <param name="status">DTO с одновленными данными статуса доски</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>
        /// Проверяет существует ли статус с переданным названием,
        /// если статуса с таким названием нет то создается новый статус, после чего обновляется статус доски
        /// </returns>
        /// <exception cref="KeyNotFoundException">Если статус не найден </exception>
        public async Task UpdateAsync(StatusDTO status, CancellationToken cancellationToken)
        {
            var updatedStatus = await _context.BoardStatuses.FindAsync(status.Id, cancellationToken);
            if (updatedStatus is null)
                throw new KeyNotFoundException();

            var existedStatus = await _context.Statuses.FirstOrDefaultAsync(s => s.Name == status.Name, cancellationToken);
            if (existedStatus is null)
            {
                var newStatus = status.Adapt<Status>(config.Fork(f => f.ForType<StatusDTO, Status>().Ignore("Id")));
                await _context.Statuses.AddAsync(newStatus, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                existedStatus = newStatus;
            }
            updatedStatus.StatusId = existedStatus.Id;
            await _context.SaveChangesAsync(cancellationToken);
        }
        /// <summary>
        /// Создать стандартные статусы доски 
        /// </summary>
        /// <param name="baseBoardId">ID базовой доски</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <remarks>
        /// Создает стандартные статусы для переданной базовой доски, а именно:
        /// новые, в работе, проверяются, готовые
        /// </remarks>
        public async Task CreateBaseStatusesAsync(int baseBoardId, CancellationToken cancellationToken)
        {
            List<BoardStatus> newBoardStatuses = [
                new BoardStatus{ BaseBoardId = baseBoardId, StatusId = 1 },
                new BoardStatus{ BaseBoardId = baseBoardId, StatusId = 2 },
                new BoardStatus{ BaseBoardId = baseBoardId, StatusId = 3 },
                new BoardStatus{ BaseBoardId = baseBoardId, StatusId = 4 }];
            await _context.BoardStatuses.AddRangeAsync(newBoardStatuses, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
