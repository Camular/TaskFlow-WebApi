namespace TaskFlow.WebApi.Core.Entities
{
    public class Permission
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public required string Description { get; set; }
    }
}
