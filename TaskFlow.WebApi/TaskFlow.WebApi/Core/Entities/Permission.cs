namespace TaskFlow.WebApi.Core.Entities
{
    public class Permission
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        public required string Description { get; set; }

        // ICollection  yok çünkü "Bana 'taskitem.create' iznini getir ve bu izne sahip olan tüm rolleri listele" isteğinde bulunmıcaz.
    }
}
