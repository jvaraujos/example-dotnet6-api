using Example.Domain.Entities.Identities;

namespace Example.Application.Contracts.Identity
{
    public interface IRefreshTokenService
    {
        Task<bool> AddAsync(RefreshToken refreshToken);
        Task<RefreshToken> GetRefreshTokenByToken(string refreshToken);
        Task DeleteExpiredTokens();
    }
}
