using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Example.Domain.Entities.Identities;

namespace Example.Identity
{
    public class ExampleIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public ExampleIdentityDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
