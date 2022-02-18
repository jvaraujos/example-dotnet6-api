using Microsoft.AspNetCore.Identity;

namespace Example.Identity.Seed
{
    public static class UserCreator
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            var applicationUser = new ApplicationUser
            {
                FirstName = "Maria Joaquina",
                LastName = "Amaral Pereira Gloria",
                UserName = "userTest",
                Email = "test@hotmail.com",
                EmailConfirmed = true
            };

            var user = await userManager.FindByEmailAsync(applicationUser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(applicationUser, "BancoDev@123");
            }
        }
    }
}