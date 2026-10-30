using Microsoft.EntityFrameworkCore;
using TaskFlow.WebApi.Core.Authorization;
using TaskFlow.WebApi.Core.DTOs.Board;
using TaskFlow.WebApi.Core.Entities;
using TaskFlow.WebApi.Core.Exceptions;
using TaskFlow.WebApi.Core.Interfaces;
using TaskFlow.WebApi.Infrastructure.Data;

namespace TaskFlow.WebApi.Infrastructure.Services
{
    public class BoardService : IBoardService
    {
        private readonly AppDbContext _context;

        public BoardService(AppDbContext context)
        {
            _context = context;
        }


        // ====================================================================================================
        // Guards
        // Shared workspace access check, board lookup, and column lookup used by the actions below.
        // ====================================================================================================

        private async Task<UserWorkspaceRole> CheckAndGetWorkspaceRoleAsync(Guid userId, Guid workspaceId, string permissionCode)
        {
            var userWorkspaceRole = await _context.UserWorkspaceRoles
                .Include(uwr => uwr.Workspace)
                .Include(uwr => uwr.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(uwr => uwr.UserId == userId && uwr.WorkspaceId == workspaceId);

            if (userWorkspaceRole == null)
            {
                throw new KeyNotFoundException("Workspace was not found, or you do not have access to it.");
            }

            var hasPermission = userWorkspaceRole.Role.RolePermissions
                .Any(rp => rp.Permission.Code == permissionCode);

            if (!hasPermission)
            {
                throw new ForbiddenException($"You do not have the '{permissionCode}' permission required for this operation.");
            }

            return userWorkspaceRole;
        }

        private async Task<Board> GetBoardInWorkspaceAsync(Guid workspaceId, Guid boardId)
        {
            var board = await _context.Boards
                .FirstOrDefaultAsync(b => b.WorkspaceId == workspaceId && b.Id == boardId);

            if (board == null)
            {
                throw new KeyNotFoundException("The board was not found in this workspace.");
            }

            return board;
        }

        private async Task<Column> GetColumnOnBoardAsync(Guid boardId, Guid columnId)
        {
            var column = await _context.Columns
                .FirstOrDefaultAsync(c => c.BoardId == boardId && c.Id == columnId);

            if (column == null)
            {
                throw new KeyNotFoundException("The column was not found on this board.");
            }

            return column;
        }


        // ====================================================================================================
        // Board queries
        // List boards in a workspace, and get one board by id.
        // ====================================================================================================

        public async Task<List<BoardSummaryDto>> GetWorkspaceBoardsAsync(Guid userId, Guid workspaceId)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.TaskItem.Read);

            return await _context.Boards
                .AsNoTracking()
                .Where(b => b.WorkspaceId == workspaceId)
                .OrderBy(b => b.CreatedAt)
                .Select(b => new BoardSummaryDto
                {
                    Id = b.Id,
                    WorkspaceId = b.WorkspaceId,
                    Name = b.Name,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<BoardSummaryDto> GetBoardByIdAsync(Guid userId, Guid workspaceId, Guid boardId)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.TaskItem.Read);

            var board = await _context.Boards
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.WorkspaceId == workspaceId && b.Id == boardId);

            if (board == null)
            {
                throw new KeyNotFoundException("The board was not found in this workspace.");
            }

            return new BoardSummaryDto
            {
                Id = board.Id,
                WorkspaceId = board.WorkspaceId,
                Name = board.Name,
                CreatedAt = board.CreatedAt
            };
        }


        // ====================================================================================================
        // Board commands
        // Create, update, and delete a board in a workspace.
        // ====================================================================================================

        public async Task<BoardSummaryDto> CreateBoardAsync(Guid userId, Guid workspaceId, CreateBoardRequest request)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.TaskItem.Update);

            var boardNameExists = await _context.Boards
                .AnyAsync(b => b.WorkspaceId == workspaceId && b.Name.ToLower() == request.Name.ToLower());

            if (boardNameExists)
            {
                throw new ConflictException($"A board named '{request.Name}' already exists in this workspace.");
            }

            var newBoard = new Board
            {
                Id = Guid.NewGuid(),
                WorkspaceId = workspaceId,
                Name = request.Name,
                CreatedAt = DateTime.UtcNow,
                Workspace = null!
            };

            await _context.Boards.AddAsync(newBoard);
            await _context.SaveChangesAsync();

            return new BoardSummaryDto
            {
                Id = newBoard.Id,
                WorkspaceId = newBoard.WorkspaceId,
                Name = newBoard.Name,
                CreatedAt = newBoard.CreatedAt
            };
        }

        public async Task<BoardSummaryDto> UpdateBoardAsync(Guid userId, Guid workspaceId, Guid boardId, UpdateBoardRequest request)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.TaskItem.Update);

            var board = await GetBoardInWorkspaceAsync(workspaceId, boardId);

            var boardNameExists = await _context.Boards
                .AnyAsync(b =>
                    b.WorkspaceId == workspaceId &&
                    b.Id != boardId &&
                    b.Name.ToLower() == request.Name.ToLower());

            if (boardNameExists)
            {
                throw new ConflictException($"A board named '{request.Name}' already exists in this workspace.");
            }

            board.Name = request.Name;
            await _context.SaveChangesAsync();

            return new BoardSummaryDto
            {
                Id = board.Id,
                WorkspaceId = board.WorkspaceId,
                Name = board.Name,
                CreatedAt = board.CreatedAt
            };
        }

        public async Task<bool> DeleteBoardAsync(Guid userId, Guid workspaceId, Guid boardId)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.TaskItem.Update);

            var board = await GetBoardInWorkspaceAsync(workspaceId, boardId);

            _context.Boards.Remove(board);
            await _context.SaveChangesAsync();

            return true;
        }


        // ====================================================================================================
        // Column queries
        // List columns on a board.
        // ====================================================================================================

        public async Task<List<ColumnSummaryDto>> GetBoardColumnsAsync(Guid userId, Guid workspaceId, Guid boardId)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.TaskItem.Read);

            await GetBoardInWorkspaceAsync(workspaceId, boardId);

            return await _context.Columns
                .AsNoTracking()
                .Where(c => c.BoardId == boardId)
                .OrderBy(c => c.Position)
                .Select(c => new ColumnSummaryDto
                {
                    Id = c.Id,
                    BoardId = c.BoardId,
                    Name = c.Name,
                    Position = c.Position
                })
                .ToListAsync();
        }


        // ====================================================================================================
        // Column commands
        // Create, update, and delete a column on a board.
        // ====================================================================================================

        public async Task<ColumnSummaryDto> CreateColumnAsync(Guid userId, Guid workspaceId, Guid boardId, CreateColumnRequest request)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.TaskItem.Update);

            await GetBoardInWorkspaceAsync(workspaceId, boardId);

            var columnNameExists = await _context.Columns
                .AnyAsync(c => c.BoardId == boardId && c.Name.ToLower() == request.Name.ToLower());

            if (columnNameExists)
            {
                throw new ConflictException($"A column named '{request.Name}' already exists on this board.");
            }

            var newColumn = new Column
            {
                Id = Guid.NewGuid(),
                BoardId = boardId,
                Name = request.Name,
                Position = request.Position,
                Board = null!
            };

            await _context.Columns.AddAsync(newColumn);
            await _context.SaveChangesAsync();

            return new ColumnSummaryDto
            {
                Id = newColumn.Id,
                BoardId = newColumn.BoardId,
                Name = newColumn.Name,
                Position = newColumn.Position
            };
        }

        public async Task<ColumnSummaryDto> UpdateColumnAsync(Guid userId, Guid workspaceId, Guid boardId, Guid columnId, UpdateColumnRequest request)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.TaskItem.Update);

            await GetBoardInWorkspaceAsync(workspaceId, boardId);

            var column = await GetColumnOnBoardAsync(boardId, columnId);

            var columnNameExists = await _context.Columns
                .AnyAsync(c =>
                    c.BoardId == boardId &&
                    c.Id != columnId &&
                    c.Name.ToLower() == request.Name.ToLower());

            if (columnNameExists)
            {
                throw new ConflictException($"A column named '{request.Name}' already exists on this board.");
            }

            column.Name = request.Name;
            column.Position = request.Position;
            await _context.SaveChangesAsync();

            return new ColumnSummaryDto
            {
                Id = column.Id,
                BoardId = column.BoardId,
                Name = column.Name,
                Position = column.Position
            };
        }

        public async Task<bool> DeleteColumnAsync(Guid userId, Guid workspaceId, Guid boardId, Guid columnId)
        {
            await CheckAndGetWorkspaceRoleAsync(userId, workspaceId, SystemPermissions.TaskItem.Update);

            await GetBoardInWorkspaceAsync(workspaceId, boardId);

            var column = await GetColumnOnBoardAsync(boardId, columnId);

            _context.Columns.Remove(column);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
