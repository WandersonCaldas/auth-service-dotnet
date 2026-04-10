namespace AuthService.API.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiresAt { get; set; }
        public int ProfileId { get; set; }

        public Profile Profile { get; set; }
    }
}
