using talker.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using talker.Domain.Entities;
using System.Collections.Generic;
using System;

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
                UserName = "a@a.pl",
                Email = "a@a.pl",
                FirstName = "Amadro",
                LastName = "Ordama",
                PictureUrl = "../assets/t.png",
            };

            if (userManager.Users.All(u => u.UserName != administrator.UserName))
            {
                await userManager.CreateAsync(administrator, "Admin123$");
                await userManager.AddToRolesAsync(administrator, new [] { administratorRole.Name });
            }


            var user = new ApplicationUser()
            {
                UserName = "b@b.pl",
                Email = "b@b.pl",
                FirstName = "Leoksandra",
                LastName = "Sokół",
                PictureUrl = "https://www.gravatar.com/avatar/b9b9074d4d7af0c2399fb173ccb431c4?d=retro&s=200",
            };

            if (userManager.Users.All(u => u.UserName != user.UserName))
            {
                await userManager.CreateAsync(user, "Admin123$");
            }

            var user1 = new ApplicationUser()
            {
                UserName = "c@c.pl",
                Email = "c@c.pl",
                FirstName = "Jan",
                LastName = "Nowak",
                PictureUrl = "https://www.gravatar.com/avatar/49b4cbe775877ab251e131ef0ff3100f?d=retro&s=200",
            };

            if (userManager.Users.All(u => u.UserName != user1.UserName))
            {
                await userManager.CreateAsync(user1, "Admin123$");
            }

            var user2 = new ApplicationUser()
            {
                UserName = "d@d.pl",
                Email = "d@d.pl",
                FirstName = "John",
                LastName = "Doe",
                PictureUrl = "https://www.gravatar.com/avatar/f19badc9694d312d1900d332fafd3ebd?d=retro&s=200",
            };

            if (userManager.Users.All(u => u.UserName != user2.UserName))
            {
                await userManager.CreateAsync(user2, "Admin123$");
            }

        }

        public static async Task SeedSampleDataAsync(ApplicationDbContext context)
        {
            if (!context.Conversations.Any() && !context.Messages.Any())
            {
                var users = context.Users.ToList();
                var userIds = new List<UserConversation>();

                string adminId = users.Where(u => u.Email == "a@a.pl").FirstOrDefault().Id;

                var messages = new List<Message>(){
                    new Message
                    {
                        Content = "Talker Hello World!",
                        Created = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now),
                        CreatedBy = adminId,
                    }
                };

                users.ForEach(u =>
                {
                    userIds.Add(new UserConversation
                    {
                        UserId = u.Id,
                        Role = (u.Id == adminId)?
                                Domain.Enums.ConversationRole.Creator:
                                Domain.Enums.ConversationRole.Admin,
                        Created = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now),
                        CreatedBy = adminId,
                    });
                    if (u.Id != adminId)
                    {
                        var affectedUser = $"{u.FirstName} {u.LastName}";
                        
                        messages.Add(new Message
                        {
                            Content = $"added {affectedUser} to the conversation",
                            Type = Domain.Enums.MessageType.AddedUserToConversation,
                            Created = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now),
                            CreatedBy = adminId,
                        });

                        messages.Add(new Message
                        {
                            Content = $"added {affectedUser} as a group admin",
                            Type = Domain.Enums.MessageType.AdminGranted,
                            Created = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now),
                            CreatedBy = adminId,
                        });
                    }
                });


                context.Conversations.Add(new Conversation
                {
                    Name = "All Talker users",
                    Color = Domain.Enums.ConversationColor.Pride,
                    UsersIds = userIds,
                    Messages = messages,
                    Created = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now),
                    CreatedBy = adminId,
                });

                context.SaveChanges();
            }
        }
    }
}
