namespace MyNewApp.Domain.Entities
{
    public class RefreshToken : BaseEntities
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public string? CreatedByIp { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? RevokedByIp { get; set; }
        public bool IsRevoked { get; set; }

        //Foreign key navigation property to User
        public User User { get; set; } = null!;
    }
}
