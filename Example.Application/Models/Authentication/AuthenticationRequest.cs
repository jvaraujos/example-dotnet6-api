namespace Example.Application.Models.Authentication
{
    public class AuthenticationRequest
    {
        public string GrantType { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
    }
}
