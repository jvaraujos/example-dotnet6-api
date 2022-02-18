using Audit.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Example.Domain.Entities.Logs;
using System.Reflection;

namespace Example.Persistence.DbContexts
{
    public class ExampleDbContext : AuditDbContext
    {
        public ExampleDbContext(DbContextOptions<ExampleDbContext> options) : base(options)
        {

        }

        public virtual DbSet<LogErro> LogErros { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            //Remove delete cascade
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }
}
