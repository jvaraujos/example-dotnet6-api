using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Example.Application.Contracts.Persistence.Interfaces.Logs;
using Example.Persistence.DbContexts;
using Example.Persistence.Repositories;

namespace Example.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DbContexts.ExampleDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("ExampleContext"), sqloption =>
                {
                    sqloption.EnableRetryOnFailure(
                        10, TimeSpan.FromSeconds(5), null);
                });
            });

            Error(services);
            return services;
        }

        private static void Error(IServiceCollection services)
        {
            services.AddScoped<ILogErroRepository, LogErroRepository>();
        }
    }
}
