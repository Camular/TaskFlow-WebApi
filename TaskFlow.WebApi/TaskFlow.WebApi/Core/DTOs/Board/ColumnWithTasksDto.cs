using TaskFlow.WebApi.Core.DTOs.Task;

namespace TaskFlow.WebApi.Core.DTOs.Board
{
    public class ColumnWithTasksDto
    {
        public Guid Id { get; set; }
        public Guid BoardId { get; set; }
        public required string Name { get; set; }
        public int Position { get; set; }
        public List<TaskSummaryDto> Tasks { get; set; } = new List<TaskSummaryDto>();
    }
}
