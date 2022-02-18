using Microsoft.EntityFrameworkCore;
using Example.Application.Contracts.Identity;
using Example.Domain.Entities.Identities;

namespace Example.Identity.Services
{
    public class RefreshTokenService :IRefreshTokenService
    {
        private readonly ExampleIdentityDbContext _dbContext;

        public RefreshTokenService(ExampleIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> AddAsync(RefreshToken refreshToken)
        {
            await _dbContext.Set<RefreshToken>().AddAsync(refreshToken);
            var affectedRows = await _dbContext.SaveChangesAsync();
            return affectedRows > 0;
        }
        public async Task DeleteExpiredTokens()
        {
            DbSet<RefreshToken> refreshTokenDbContex = _dbContext.Set<RefreshToken>();

            List<RefreshToken> listToBeDeleted = await refreshTokenDbContex.Where(x => x.ExpiredIn < DateTime.UtcNow)
                .ToListAsync();

            refreshTokenDbContex.RemoveRange(listToBeDeleted);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<RefreshToken> GetRefreshTokenByToken(string refreshToken)
        {
            var result = await _dbContext.Set<RefreshToken>()
                                 .Where(p => p.Token.ToString() == refreshToken)
                                 .FirstOrDefaultAsync();
            return result;

        }
    }
}
