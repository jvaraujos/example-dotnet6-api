using Microsoft.AspNetCore.Identity;

namespace Example.Api
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            if (args.Contains("seed"))
            {
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                         await Identity.Seed.UserCreator.SeedAsync(userManager);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("An error occured while starting the application");
                    }
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
