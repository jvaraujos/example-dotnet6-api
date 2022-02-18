using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Example.Application.Contracts.Identity;
using Example.Application.Models.Authentication;

namespace Example.Api.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : Controller
    {
        
        private readonly IAuthenticationService _authenticationService;

        public AccountController(IAuthenticationService authService)
        {
            _authenticationService = authService;
        }

        [HttpPost("authenticate/azuread"), AllowAnonymous]
        public async Task<IActionResult> AuthenticateWithAzureAd([FromBody] AuthenticationAzureAdRequest authenticationAzureAdRequest)
        {
            var authenticatedUser = await _authenticationService.AuthenticateWithAzureAdAsync(authenticationAzureAdRequest.AccessToken);

            if (authenticatedUser == null)
            {
                return Unauthorized();
            }

            return Ok(authenticatedUser);
        }

        [HttpPost("authenticate"), AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticationRequest authenticationRequest)
        {
            AuthenticationResponse authenticationResponse;
            switch (authenticationRequest.GrantType)
            {
                case "refresh_token":
                    authenticationResponse = await _authenticationService.AuthenticateWithRefreshTokenAsync(authenticationRequest.RefreshToken);
                    break;
                case "password":
                    authenticationResponse =
                        await _authenticationService.AuthenticateWithPasswordAsync(authenticationRequest);
                    break;
                default: return BadRequest("Invalid grant type");
            }

            if (authenticationResponse == null)
            {
                return Unauthorized();
            }

            return Ok(authenticationResponse);
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegistrationResponse>> RegisterAsync(RegistrationRequest request)
        {
            return Ok(await _authenticationService.RegisterAsync(request));
        }

    }
}
