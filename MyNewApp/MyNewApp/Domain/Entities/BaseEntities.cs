namespace MyNewApp.Domain.Entities
{
    public class BaseEntities
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class AuditableEntities : BaseEntities
    {
        public long? CreatedBy { get; set; }
    }
}
