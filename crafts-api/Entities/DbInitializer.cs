
using crafts_api.context;
using crafts_api.Entities.Domain;
using crafts_api.Entities.Enum;
using crafts_api.models.domain;
using crafts_api.models.models;
using Microsoft.AspNetCore.Identity;

namespace crafts_api.models
{
    public class DbInitializer
    {
        public static async Task Seed(DatabaseContext context, UserManager<IdentityUser> userManager)
        {
            if (!context.Users.Any())
            {
                var registerRequest = new RegisterUserRequest
                {
                    Username = "marek123",
                    FirstName = "Marek",
                    LastName = "Marekovic",
                    Password = "marek123",
                    PasswordConfirmation = "marek123",
                    Email = "marek@gmail.com",
                    ProfilePicture = "https://i.pinimg.com/originals/0b/7b/7b/0b7b7",
                    Country = "Slovakia",
                    City = "Bratislava",
                    Address = "Karloveska 63",
                    Street = "Karloveska",
                    Number = "63",
                    PostalCode = "84104",
                    PhoneNumber = "0901234567"
                };

                var identityUser = new IdentityUser
                {
                    UserName = registerRequest.Username,
                    Email = registerRequest.Email
                };

                var userProfile = new UserProfile
                {
                    ProfilePicture = registerRequest.ProfilePicture,
                    Country = registerRequest.Country,
                    City = registerRequest.City,
                    Address = registerRequest.Address,
                    Street = registerRequest.Street,
                    Number = registerRequest.Number,
                    PostalCode = registerRequest.PostalCode,
                    PhoneNumber = registerRequest.PhoneNumber
                };

                var user = new User
                {
                    PublicId = Guid.NewGuid(),
                    IdentityId = identityUser.Id,
                    Username = registerRequest.Username,
                    FirstName = registerRequest.FirstName,
                    LastName = registerRequest.LastName,
                    Email = registerRequest.Email,
                    CreatedAt = System.DateTime.Now,
                    UpdatedAt = System.DateTime.Now,
                    Role = Role.User,
                    UserProfile = userProfile
                };

                using (var transaction = context.Database.BeginTransaction())
                {
                    var result = await userManager.CreateAsync(identityUser, registerRequest.Password);

                    if (result.Succeeded)
                    {
                        await context.Users.AddAsync(user);
                        await context.SaveChangesAsync();
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }
                }

            }

            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Woodworking",
                        SkName = "Drevo"
                    },
                    new Category
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Knitting",
                        SkName = "Pletenie"
                    },
                    new Category
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Sewing",
                        SkName = "Šitie"
                    },
                    new Category
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Pottery",
                        SkName = "Keramika"
                    },
                    new Category
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Painting",
                        SkName = "Maľovanie"
                    },
                    new Category
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Jewelry",
                        SkName = "Šperky"
                    },
                    new Category
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Glass",
                        SkName = "Sklo"
                    },
                    new Category
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Metalworking",
                        SkName = "Kováčstvo"
                    },
                    new Category
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Paper",
                        SkName = "Papier"
                    },
                    new Category
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Leather",
                        SkName = "Koža"
                    }
                };
                context.Categories.AddRange(categories);
                context.SaveChanges();
            }
        }
    }
}
