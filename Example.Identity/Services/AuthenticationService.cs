using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Example.Application.Contracts.Identity;
using Example.Application.Exceptions;
using Example.Application.Models.Authentication;
using Example.Domain.Entities.Identities;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace Example.Identity.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IConfiguration _configuration;
        private readonly ISigningKeyConfiguration _signingKeyConfiguration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;
        public AuthenticationService(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IOptions<JwtSettings> jwtSettings,
            IRefreshTokenService refreshTokenRepository,
            ILogger<AuthenticationService> logger,
            IConfiguration configuration,
            ISigningKeyConfiguration signingKeyConfiguration)
        {
            _refreshTokenService = refreshTokenRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _configuration = configuration;
            _signingKeyConfiguration = signingKeyConfiguration;
            _logger = logger;
            _tokenHandler = new JwtSecurityTokenHandler();

        }

        #region Criptography
        public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

        public bool VerifyPassword(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);

        private string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public Task<string> HashPbkdf2(string password, byte[] salt, int iterations)
        {
            string completeHash = null;
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            try
            {
                byte[] bytes = pbkdf2.GetBytes(32);

                string saltString = Encoding.UTF8.GetString(salt);
                string hashString = Convert.ToBase64String(bytes);
                completeHash = $"pbkdf2_sha256${iterations}${saltString}${hashString}";
                return Task.FromResult(completeHash);
            }
            catch (Exception)
            {
                return Task.FromResult(completeHash);
            }
        }
        #endregion

        #region Claims

        private async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            var claimsUser = await GetClaimsByUserAsync(user);
            var rolesUser = await GetRolesByUserAsync(user);

            var claims = new[]
           {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
           .Union(claimsUser)
           .Union(rolesUser);

            return claims;
        }

        private async Task<List<Claim>> GetClaimsByUserAsync(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            return userClaims.ToList();
        }

        public async Task<IList<Claim>> GetRolesByUserAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }
            return roleClaims.ToList();
        }

        #endregion

        #region Tokens

        private async Task<RefreshToken> GenerateRefreshTokenAsync(string userId)
        {
            RefreshToken refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = Guid.NewGuid(),
                CreateAt = DateTime.UtcNow,
                ExpiredIn = DateTime.UtcNow.AddSeconds(_jwtSettings.RefreshTokenExpiresIn),
            };

            bool created = await _refreshTokenService.AddAsync(refreshToken);
            await _refreshTokenService.DeleteExpiredTokens();

            return created ? refreshToken : null;
        }

        public Task<bool> VerifiyToken(string token)
        {
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = _signingKeyConfiguration.SecurityKey,
                ValidateIssuerSigningKey = true,
                ValidateAudience = true
            };
            if (!_tokenHandler.CanReadToken(token))
            {
                return Task.FromResult(false);
            }

            try
            {
                ClaimsPrincipal claimsPrincipal = _tokenHandler.ValidateToken(token, validationParameters, out _);
                return Task.FromResult(claimsPrincipal.Claims.Any());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return Task.FromResult(false);
        }

        public JwtToken GenerateToken(IEnumerable<Claim> claims, int? expiresIn = null)
        {
            ClaimsIdentity identity = new ClaimsIdentity(
                new GenericIdentity(Guid.NewGuid().ToString(), "TokenIdentifier"), claims);

            DateTime expirationDate = DateTime.UtcNow.AddSeconds(expiresIn ?? _jwtSettings.DefaultExpiresIn);

            var signingCredentials = new SigningCredentials(_signingKeyConfiguration.SecurityKey, SecurityAlgorithms.HmacSha256);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _jwtSettings.Audience,
                Issuer = _jwtSettings.Issuer,
                Expires = expirationDate,
                NotBefore = DateTime.Now,
                Subject = identity,
                SigningCredentials = signingCredentials,
            };

            SecurityToken securityToken = _tokenHandler.CreateToken(tokenDescriptor);

            return new JwtToken
            {
                Token = _tokenHandler.WriteToken(securityToken),
                CreatedAt = DateTime.UtcNow,
                ExpirationDate = expirationDate,
            };
        }

        public async Task<ApplicationUser> GetUserFromTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token) || !_tokenHandler.CanReadToken(token))
                throw new InvalidOperationException();

            List<Claim> claims = _tokenHandler.ReadJwtToken(token)?.Claims.ToList();

            if (claims == null)
                throw new InvalidOperationException();

            string claimPessoaId = claims.FirstOrDefault(w => w.Type == "userId")?.Value;

            return await _userManager.FindByIdAsync(claimPessoaId);
        }

        #endregion

        #region Authenticate

        public async Task<AuthenticationResponse> AuthenticateAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new Exception($"Usuario não encontrado.");
            }

            var claims = await GetClaimsAsync(user);

            JwtToken token = GenerateToken(claims);
            RefreshToken refreshToken = await GenerateRefreshTokenAsync(user.Id);

            if (refreshToken == null || token == null)
            {
                throw new Exception("Error creating token");
            }

            return new AuthenticationResponse(user, token, refreshToken.Token);


        }

        public async Task<AuthenticationResponse> AuthenticateWithAzureAdAsync(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                return null;
            }

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage httpResponse = await httpClient.GetAsync(_configuration["AzureAd:GraphApiURI"]);
            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            string responseContent = await httpResponse.Content.ReadAsStringAsync();
            AzureAdUser adUser = JsonConvert.DeserializeObject<AzureAdUser>(responseContent);

            string adUserMail = adUser.Mail.ToLowerInvariant();

            var applicationUser = await _userManager.FindByEmailAsync(adUserMail);

            if (applicationUser != null)
            {
                await _userManager.UpdateAsync(applicationUser);
            }
            else
            {
                var newUser = new ApplicationUser()
                {
                    Email = adUserMail,
                    UserName = adUser.DisplayName,
                };

                var identityResult = await _userManager.CreateAsync(newUser);
                if (!identityResult.Succeeded)
                    return null;
            }

            return await AuthenticateAsync(applicationUser.Id);
        }

        public async Task<AuthenticationResponse> AuthenticateWithPasswordAsync(AuthenticationRequest authenticationRequest)
        {
            var user = await _userManager.FindByEmailAsync(authenticationRequest.Email);

            if (user == null)
            {
                throw new Exception($"User with {authenticationRequest.Email} not found.");
            }
            var result = await _signInManager.PasswordSignInAsync(user.UserName, authenticationRequest.Password, false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                throw new Exception($"Credentials for '{authenticationRequest.Email} aren't valid'.");
            }

            return await AuthenticateAsync(user.Id);
        }

        public async Task<AuthenticationResponse> AuthenticateWithRefreshTokenAsync(string token)
        {
            RefreshToken refreshToken = await _refreshTokenService.GetRefreshTokenByToken(token);

            if (refreshToken == null || DateTime.UtcNow > refreshToken.ExpiredIn)
            {
                return null;
            }

            var user = await _userManager.FindByIdAsync(refreshToken.UserId);

            if (user == null)
            {
                return null;
            }

            return await AuthenticateAsync(user.Id);
        }

        #endregion

        #region Register
        public async Task<RegistrationResponse> RegisterAsync(RegistrationRequest request)
        {
            var existingUser = await _userManager.FindByNameAsync(request.UserName);

            if (existingUser != null)
            {
                throw new Exception($"UserName '{request.UserName}' already exists.");
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                EmailConfirmed = true
            };

            var existingEmail = await _userManager.FindByEmailAsync(request.Email);

            if (existingEmail == null)
            {
                var result = await _userManager.CreateAsync(user, request.Password);

                if (result.Succeeded)
                {
                    return new RegistrationResponse() { UserId = user.Id };
                }
                else
                {
                    throw new IdentityException(result.Errors);
                }
            }
            else
            {
                throw new Exception($"Email {request.Email } already exists.");
            }
        }

        #endregion
    }

}
