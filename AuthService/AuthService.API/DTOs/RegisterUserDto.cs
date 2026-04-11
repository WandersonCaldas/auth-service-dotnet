namespace AuthService.API.DTOs
{
    public class RegisterUserDto
    {
        public string Email { get; set; }

        public string Password { get; set; }
        public int ProfileId { get; set; }
    }    
}
