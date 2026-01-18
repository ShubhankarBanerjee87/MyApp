namespace MyNewApp.Domain.Entities
{
    public class BaseEntities
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public long? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long? UpdatedBy { get; set; }    
        public bool IsActive { get; set; } = true;
    }
}
