using Example.Api.Bootstrapper;
using Example.Api.Middleware;
using Example.Identity;
using Example.Persistence;

namespace Example.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddPersistenceServices(Configuration);
            services.AddIdentityServices(Configuration);
            services.ConfigHelthCheck(Configuration);
            services.ConfigSwagger();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.StartHelthCheck(env.EnvironmentName);
            app.UseCustomExceptionHandler();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors(options =>
            {
                options.AllowAnyOrigin();
                options.AllowAnyHeader();
                options.WithMethods("POST", "GET", "PUT", "DELETE");
            });
            app.UseRouting();
            app.StartSwagger();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
