namespace TaskFlow.WebApi.Core.DTOs.Board
{
    public class ColumnSummaryDto
    {
        public Guid Id { get; set; }
        public Guid BoardId { get; set; }
        public required string Name { get; set; }
        public int Position { get; set; }
    }
}
