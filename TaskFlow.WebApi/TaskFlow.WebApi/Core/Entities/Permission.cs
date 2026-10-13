namespace TaskFlow.WebApi.Core.Entities
{
    public class Permission
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public required string Description { get; set; }

        // No collection navigation. Callers load a permission by code; they do not list every role that has it.
    }
}
