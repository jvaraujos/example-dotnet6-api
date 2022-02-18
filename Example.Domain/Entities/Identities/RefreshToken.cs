namespace Example.Domain.Entities.Identities
{
    public class RefreshToken
    {
        public Guid RefreshTokenId { get; set; }
        public Guid Token { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime ExpiredIn { get; set; }
        public DateTime CreateAt { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
