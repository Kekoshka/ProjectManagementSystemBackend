using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystemBackend.Context;
using ProjectManagementSystemBackend.Interfaces;
using ProjectManagementSystemBackend.Models;
using ProjectManagementSystemBackend.Models.DTO;
using ProjectManagementSystemBackend.Common.CustomExceptions;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystemBackend.Services
{
    /// <summary>
    /// Сервис для работы с досками проектов (Kanban и Scrum)
    /// </summary>
    /// <remarks>
    /// Предоставляет функционал для создания, получения, обновления и удаления досок проектов.
    /// Поддерживает два типа досок: Kanban и Scrum.
    /// </remarks>
    public class BoardService : IBoardService
    {
        ApplicationContext _context;
        IStatusService _statusService;
        TypeAdapterConfig config = new();

        /// <summary>
        /// Конструктор сервиса досок
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        /// <param name="statusService">Сервис для работы со статусами</param>
        public BoardService(ApplicationContext context, IStatusService statusService) 
        {
            _context = context;
            _statusService = statusService;
        }

        /// <summary>
        /// Получить все базовые доски проекта
        /// </summary>
        /// <param name="projectId">ID проекта</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Список базовых досок проекта</returns>
        public async Task<IEnumerable<BaseBoardDTO>> GetBoardsByProjectIdAsync(int projectId, CancellationToken cancellationToken)
        {
            var boards = await _context.BaseBoards
                .Where(b => b.ProjectId == projectId)
                .ProjectToType<BaseBoardDTO>()
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return boards;
        }

        /// <summary>
        /// Получить доску по ID базовой доски
        /// </summary>
        /// <param name="baseBoardId">ID базовой доски</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>
        /// Объект доски (ScrumBoardDTO или KanbanBoardDTO), 
        /// либо null если доска не найдена
        /// </returns>
        public async Task<object?> GetByBaseBoardIdAsync(int baseBoardId, CancellationToken cancellationToken)
        {
            var scrumBoard = await _context.ScrumBoards
                .AsNoTracking()
                .ProjectToType<ScrumBoardDTO>()
                .FirstOrDefaultAsync(sb => sb.BaseBoardId == baseBoardId, cancellationToken);
            if (scrumBoard is not null)
                return scrumBoard;

            var kanbanBoard = await _context.KanbanBoards
                .AsNoTracking()
                .ProjectToType<KanbanBoardDTO>()
                .FirstOrDefaultAsync(cb => cb.BaseBoardId == baseBoardId, cancellationToken);
            return kanbanBoard is null ? null : kanbanBoard;
        }

        /// <summary>
        /// Создать новую Kanban доску
        /// </summary>
        /// <param name="kanbanBoard">DTO с данными для создания Kanban доски</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Созданная Kanban доска</returns>
        /// <remarks>
        /// Создает:
        /// 1. Базовую доску
        /// 2. Kanban доску
        /// 3. Базовые статусы для доски
        /// </remarks>
        public async Task<KanbanBoardDTO> PostKanbanAsync(KanbanBoardDTO kanbanBoard, CancellationToken cancellationToken)
        {
            var newBaseBoard = kanbanBoard.BaseBoard.Adapt<BaseBoard>(config.Fork(f => f.ForType<BaseBoardDTO, BaseBoard>().Ignore("Id"))); 
            await _context.BaseBoards.AddAsync(newBaseBoard, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            KanbanBoard newKanbanBoard = new()
            {
                TaskLimit = kanbanBoard.TaskLimit,
                BaseBoard = newBaseBoard
            }; 
            await _context.KanbanBoards.AddAsync(newKanbanBoard,cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _statusService.CreateBaseStatusesAsync(newBaseBoard.Id, cancellationToken);

            return newKanbanBoard.Adapt<KanbanBoardDTO>();
        }

        /// <summary>
        /// Создать новую Scrum доску
        /// </summary>
        /// <param name="scrumBoard">DTO с данными для создания Scrum доски</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Созданная Scrum доска</returns>
        /// <remarks>
        /// Создает:
        /// 1. Базовую доску
        /// 2. Scrum доску
        /// 3. Базовые статусы для доски
        /// </remarks>
        public async Task<ScrumBoardDTO> PostScrumAsync(ScrumBoardDTO scrumBoard, CancellationToken cancellationToken)
        {
            BaseBoard newBaseBoard = scrumBoard.BaseBoard.Adapt<BaseBoard>(config.Fork(f => f.ForType<BaseBoardDTO, BaseBoard>().Ignore(bb => bb.Id)));
            await _context.BaseBoards.AddAsync(newBaseBoard, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            ScrumBoard newScrumBoard = new()
            {
                TimeLimit = scrumBoard.TimeLimit,
                BaseBoard = newBaseBoard
            };
            await _context.ScrumBoards.AddAsync(newScrumBoard, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _statusService.CreateBaseStatusesAsync(newBaseBoard.Id, cancellationToken);

            return newScrumBoard.Adapt<ScrumBoardDTO>();
        }

        /// <summary>
        /// Обновить Kanban доску
        /// </summary>
        /// <param name="newKanbanBoard">DTO с обновленными данными Kanban доски</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <remarks>
        /// Обновляет:
        /// 1. Параметры Kanban доски (лимит задач)
        /// 2. Данные базовой доски (название, описание)
        /// </remarks>
        /// <exception cref="NotFoundException">
        /// Если Kanban доска или базовая доска не найдены
        /// </exception>
        /// <exception cref="ConflictException">
        /// Если ID базовой доски не соответствует доске
        /// </exception>
        public async Task UpdateKanbanBoardAsync(KanbanBoardDTO newKanbanBoard, CancellationToken cancellationToken)
        {
            var kanbanBoard = await _context.KanbanBoards.FindAsync(newKanbanBoard.Id,cancellationToken);
            if (kanbanBoard is null)
                throw new NotFoundException($"Not found kanban board with {newKanbanBoard.Id} id");
            if (kanbanBoard.BaseBoardId != newKanbanBoard.BaseBoardId)
                throw new ConflictException ($"KanbanBoard doesnt have BaseBoard with {newKanbanBoard.BaseBoardId} id");
            kanbanBoard.TaskLimit = newKanbanBoard.TaskLimit;

            var baseBoard = await _context.BaseBoards.FindAsync(newKanbanBoard.BaseBoardId,cancellationToken);
            if (baseBoard is null)
                throw new NotFoundException($"Not found base board with {newKanbanBoard.BaseBoardId} id");
            baseBoard.Name = newKanbanBoard.BaseBoard.Name;
            baseBoard.Description = newKanbanBoard.BaseBoard.Description;
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Обновить Scrum доску
        /// </summary>
        /// <param name="newScrumBoard">DTO с обновленными данными Scrum доски</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <remarks>
        /// Обновляет:
        /// 1. Параметры Scrum доски (временной лимит)
        /// 2. Данные базовой доски (название, описание)
        /// </remarks>
        /// <exception cref="NotFoundException">
        /// Если Scrum доска или базовая доска не найдены
        /// </exception>
        /// <exception cref="ConflictException">
        /// Если ID базовой доски не соответствует доске
        /// </exception>
        public async Task UpdateScrumBoardAsync(ScrumBoardDTO newScrumBoard, CancellationToken cancellationToken)
        {
            var scrumBoard = await _context.ScrumBoards.FindAsync(newScrumBoard.Id,cancellationToken);
            if (scrumBoard is null)
                throw new NotFoundException($"Not found scrum board with {newScrumBoard.Id} id");
            if (scrumBoard.BaseBoardId != newScrumBoard.BaseBoardId)
                throw new ConflictException($"KanbanBoard doesnt have BaseBoard with {newScrumBoard.BaseBoardId} id");
            scrumBoard.TimeLimit = newScrumBoard.TimeLimit;

            var baseBoard = await _context.BaseBoards.FindAsync(newScrumBoard.BaseBoardId, cancellationToken);
            if (baseBoard is null)
            throw new NotFoundException($"Not found base board with {newScrumBoard.BaseBoard.Id} id");
            baseBoard.Name = newScrumBoard.BaseBoard.Name;
            baseBoard.Description = newScrumBoard.BaseBoard.Description;
            await _context.SaveChangesAsync(cancellationToken);
        }
        /// <summary>
        /// Удалить доску
        /// </summary>
        /// <param name="baseBoardId">ID базовой доски</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <remarks>
        /// Удаляет:
        /// 1. Базовую доску
        /// 2. Привязанную доску (Kanban или Scrum)
        /// </remarks>
        /// <exception cref="NotFoundException">
        /// Если базовая доска или связанные доски не найдены
        /// </exception>
        public async Task DeleteAsync(int baseBoardId, CancellationToken cancellationToken)
        {
            var baseBoard = await _context.BaseBoards.FindAsync(baseBoardId,cancellationToken);
            if (baseBoard is null)
                throw new NotFoundException("Not found base board");

            var scrumBoard = await _context.ScrumBoards.FirstOrDefaultAsync(sb => sb.BaseBoardId == baseBoardId, cancellationToken);
            var kanbanBoard = await _context.KanbanBoards.FirstOrDefaultAsync(cb => cb.BaseBoardId == baseBoardId, cancellationToken);

            if (scrumBoard is null && kanbanBoard is null)
                throw new NotFoundException("Not found scrum and Kanban boards");

            _context.BaseBoards.Remove(baseBoard);
            if (scrumBoard is null)
                _context.KanbanBoards.Remove(kanbanBoard);
            _context.ScrumBoards.Remove(scrumBoard);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
