using crafts_api.context;
using crafts_api.Entities.Domain;
using crafts_api.Entities.Enum;
using crafts_api.Entities.Models;
using crafts_api.models.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace crafts_api.Entities
{
    public class DbInitializer
    {
        public static async Task Seed(DatabaseContext context, UserManager<IdentityUser> userManager)
        {
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new ()
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Woodworking",
                        SkName = "Drevo"
                    },
                    new ()
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Knitting",
                        SkName = "Pletenie"
                    },
                    new ()
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Sewing",
                        SkName = "Šitie"
                    },
                    new ()
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Pottery",
                        SkName = "Keramika"
                    },
                    new ()
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Painting",
                        SkName = "Maľovanie"
                    },
                    new ()
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Jewelry",
                        SkName = "Šperky"
                    },
                    new ()
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Glass",
                        SkName = "Sklo"
                    },
                    new ()
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Metalworking",
                        SkName = "Kováčstvo"
                    },
                    new ()
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Paper",
                        SkName = "Papier"
                    },
                    new ()
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Leather",
                        SkName = "Koža"
                    }
                };
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }
            
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
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Role = Role.User,
                    UserProfile = userProfile
                };

                await using var transaction = await context.Database.BeginTransactionAsync();

                var result = await userManager.CreateAsync(identityUser, registerRequest.Password);
                if (result.Succeeded)
                {
                    await context.Users.AddAsync(user);
                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                else
                {
                    await transaction.RollbackAsync();
                }
            }

            if (!context.Crafters.Any())
            {
                var registerCraftsmanRequest = new RegisterCraftsmanRequest
                {
                    FirstName = "Jozef",
                    LastName = "Jozefovic",
                    Username = "jozef123",
                    Email = "jozef123@post.sk",
                    Password = "jozef123",
                    PasswordConfirmation = "jozef123",
                    Bio = "I am a craftsmen, I love to create things",
                    PhoneNumber = "0901234567",
                    City = "Bratislava",
                    Country = "Slovakia",
                    Number = "63",
                    PostalCode = "84104",
                    Street = "Karloveska"
                };

                var identityUser = new IdentityUser
                {
                    UserName = registerCraftsmanRequest.Username,
                    Email = registerCraftsmanRequest.Email
                };

                var craftsmanProfile = new CraftsmanProfile
                {
                    Bio = registerCraftsmanRequest.Bio,
                    PhoneNumber = registerCraftsmanRequest.PhoneNumber,
                    City = registerCraftsmanRequest.City,
                    Country = registerCraftsmanRequest.Country,
                    Street = registerCraftsmanRequest.Street,
                    Number = registerCraftsmanRequest.Number,
                    PostalCode = registerCraftsmanRequest.PostalCode
                };

                var craftsman = new Craftsman
                {
                    PublicId = Guid.NewGuid(),
                    IdentityId = identityUser.Id,
                    Username = registerCraftsmanRequest.Username,
                    FirstName = registerCraftsmanRequest.FirstName,
                    LastName = registerCraftsmanRequest.LastName,
                    Email = registerCraftsmanRequest.Email,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Role = Role.Crafter,
                    CraftsmanProfile = craftsmanProfile
                };
                
                var category = await context.Categories.FirstOrDefaultAsync( c => c.Name == "Woodworking");
                
                if (category == null)
                {
                    category = new Category
                    {
                        PublicId = Guid.NewGuid(),
                        Name = "Woodworking",
                        SkName = "Drevo"
                    };
                    await context.Categories.AddAsync(category);
                    await context.SaveChangesAsync();
                }
                
                var addServiceRequest = new AddServiceRequest
                {
                    Name = "Woodworking",
                    Description = "I am a craftsmen, I love to create things",
                    Price = 20,
                    Duration = 2,
                    CategoryPublicId = category.PublicId
                };
                
                var service = new Service
                {
                    PublicId = Guid.NewGuid(),
                    Name = addServiceRequest.Name,
                    Description = addServiceRequest.Description,
                    CategoryPublicId = addServiceRequest.CategoryPublicId
                };
                
                var craftsmanService = new CraftsmanService
                {
                    CraftsmanProfileCraftsmanPublicId = craftsman.PublicId,
                    ServicePublicId = service.PublicId,
                    Price = addServiceRequest.Price,
                    Duration = addServiceRequest.Duration
                };

                await using var transaction = await context.Database.BeginTransactionAsync();

                var result = await userManager.CreateAsync(identityUser, registerCraftsmanRequest.Password);

                if (result.Succeeded)
                {
                    await context.Crafters.AddAsync(craftsman);
                    await context.Services.AddAsync(service);
                    await context.CraftsmanServices.AddAsync(craftsmanService);
                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                else
                {
                    await transaction.RollbackAsync();
                }

            }
        }
    }
}
