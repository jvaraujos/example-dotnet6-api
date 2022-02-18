using Example.Domain.Entities.Identities;
using System.Text.Json.Serialization;

namespace Example.Application.Models.Authentication
{
    public class AuthenticationResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public Guid RefreshToken { get; set; }
        //public RefreshToken RefreshToken { get; set; }
        public JwtToken JwtToken { get; set; }
        public AuthenticationResponse(ApplicationUser applicationUser, JwtToken token, Guid refreshToken)
        {
            Id = applicationUser.Id;
            FirstName = applicationUser.FirstName;
            LastName = applicationUser.LastName;
            UserName = applicationUser.UserName;
            Email=applicationUser.Email;
            JwtToken = token;
            RefreshToken = refreshToken;
        }
    }
}
