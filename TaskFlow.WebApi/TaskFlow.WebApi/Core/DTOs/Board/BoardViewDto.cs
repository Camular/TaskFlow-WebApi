namespace TaskFlow.WebApi.Core.DTOs.Board
{
    public class BoardViewDto
    {
        public Guid Id { get; set; }
        public Guid WorkspaceId { get; set; }
        public required string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ColumnWithTasksDto> Columns { get; set; } = new List<ColumnWithTasksDto>();
    }
}
