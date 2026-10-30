using TaskFlow.WebApi.Core.DTOs.Board;

namespace TaskFlow.WebApi.Core.Interfaces
{
    public interface IBoardService
    {
        // Boards — list boards, get one board, create, update, and delete a board.
        Task<List<BoardSummaryDto>> GetWorkspaceBoardsAsync(Guid userId, Guid workspaceId);
        Task<BoardSummaryDto> GetBoardByIdAsync(Guid userId, Guid workspaceId, Guid boardId);
        Task<BoardSummaryDto> CreateBoardAsync(Guid userId, Guid workspaceId, CreateBoardRequest request);
        Task<BoardSummaryDto> UpdateBoardAsync(Guid userId, Guid workspaceId, Guid boardId, UpdateBoardRequest request);
        Task<bool> DeleteBoardAsync(Guid userId, Guid workspaceId, Guid boardId);

        // Columns — list columns on a board, create, update, and delete a column.
        Task<List<ColumnSummaryDto>> GetBoardColumnsAsync(Guid userId, Guid workspaceId, Guid boardId);
        Task<ColumnSummaryDto> CreateColumnAsync(Guid userId, Guid workspaceId, Guid boardId, CreateColumnRequest request);
        Task<ColumnSummaryDto> UpdateColumnAsync(Guid userId, Guid workspaceId, Guid boardId, Guid columnId, UpdateColumnRequest request);
        Task<bool> DeleteColumnAsync(Guid userId, Guid workspaceId, Guid boardId, Guid columnId);
    }
}
