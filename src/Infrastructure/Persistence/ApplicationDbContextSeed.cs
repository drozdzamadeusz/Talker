using talker.Domain.Entities;
using talker.Domain.ValueObjects;
using talker.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace talker.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var administratorRole = new IdentityRole("Administrator");

            if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
            {
                await roleManager.CreateAsync(administratorRole);
            }

            var administrator = new ApplicationUser {
                UserName = "admin",
                Email = "admin@a.pl",
                FirstName = "Dawid",
                LastName = "Suchodolski",
                PictureUrl = "https://www.gravatar.com/avatar/6f2e03e62ed8dcd2e23d4fe56b7b210c?d=retro&s=200",
            };

            if (userManager.Users.All(u => u.UserName != administrator.UserName))
            {
                await userManager.CreateAsync(administrator, "Admin123$");
                await userManager.AddToRolesAsync(administrator, new [] { administratorRole.Name });
            }


            var user = new ApplicationUser()
            {
                UserName = "amadro",
                Email = "a@a.pl",
                FirstName = "Amadeusz",
                LastName = "Drożdż",
                PictureUrl = "https://www.gravatar.com/avatar/6f2e03e62ed83cd2e23d4fe56b7b210c?d=retro&s=200",
            };

            if (userManager.Users.All(u => u.UserName != user.UserName))
            {
                await userManager.CreateAsync(user, "Admin123$");
            }

            var user1 = new ApplicationUser()
            {
                UserName = "ordama",
                Email = "b@b.pl",
                FirstName = "Jan",
                LastName = "Nowak",
                PictureUrl = "https://www.gravatar.com/avatar/6224ff3a39a4c9efa12e73cc9a194cb5?d=retro&s=200",
            };

            if (userManager.Users.All(u => u.UserName != user1.UserName))
            {
                await userManager.CreateAsync(user1, "Admin123$");
            }

        }

        public static async Task SeedSampleDataAsync(ApplicationDbContext context)
        {
            // Seed, if necessary
            if (!context.TodoLists.Any())
            {
                context.TodoLists.Add(new TodoList
                {
                    Title = "Shopping",
                    Colour = Colour.Blue,
                    Items =
                    {
                        new TodoItem { Title = "Apples", Done = true },
                        new TodoItem { Title = "Milk", Done = true },
                        new TodoItem { Title = "Bread", Done = true },
                        new TodoItem { Title = "Toilet paper" },
                        new TodoItem { Title = "Pasta" },
                        new TodoItem { Title = "Tissues" },
                        new TodoItem { Title = "Tuna" },
                        new TodoItem { Title = "Water" }
                    }
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
