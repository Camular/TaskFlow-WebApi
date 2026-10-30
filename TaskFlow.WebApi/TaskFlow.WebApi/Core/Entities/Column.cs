namespace TaskFlow.WebApi.Core.Entities
{
    public class Column
    {
        public Guid Id { get; set; }
        public Guid BoardId { get; set; }
        public required string Name { get; set; }
        public int Position { get; set; }
        public required Board Board { get; set; }

    }
}
