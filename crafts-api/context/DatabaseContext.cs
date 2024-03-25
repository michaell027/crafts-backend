using crafts_api.Entities.Domain;
using crafts_api.models.domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace crafts_api.context;

public class DatabaseContext : IdentityDbContext
{
    private IConfiguration _configuration;

    public DatabaseContext(IConfiguration configuration) => _configuration = configuration;

    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Craftsman> Crafters { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<CraftsmanProfile> CraftsmanProfiles { get; set; }
    public DbSet<CraftsmanService> CraftsmanServices { get; set; }
    public DbSet<Service> Services { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _configuration.GetConnectionString("Default");
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.PublicId).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.SkName).IsRequired();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.PublicId).IsRequired();
            entity.Property(e => e.FirstName).IsRequired();
            entity.Property(e => e.LastName).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.Username).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.Role).IsRequired().HasConversion<string>();
            entity.HasIndex(e => e.IdentityId).IsUnique();
            entity.HasOne(e => e.UserProfile)
                .WithOne(up => up.User)
                .HasForeignKey<UserProfile>(up => up.UserPublicId)
                .HasPrincipalKey<User>(u => u.PublicId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Token).IsRequired();
            entity.Property(e => e.Expires).IsRequired();
            entity.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserIdentityId)
            .HasPrincipalKey(e => e.IdentityId)
            .IsRequired();
        });

        modelBuilder.Entity<Craftsman>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.PublicId).IsRequired();
            entity.HasIndex(e => e.IdentityId);
            entity.Property(e => e.FirstName).IsRequired();
            entity.Property(e => e.LastName).IsRequired();
            entity.Property(e => e.Username).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.Role).IsRequired().HasConversion<string>();
            entity.HasOne(e => e.CraftsmanProfile)
                .WithOne(cp => cp.Craftsman)
                .HasForeignKey<CraftsmanProfile>(cp => cp.CraftsmanPublicId)
                .HasPrincipalKey<Craftsman>(c => c.PublicId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.ProfilePicture).IsRequired();
            entity.Property(e => e.Country).IsRequired();
            entity.Property(e => e.City).IsRequired();
            entity.Property(e => e.Address).IsRequired();
            entity.Property(e => e.Street).IsRequired();
            entity.Property(e => e.Number).IsRequired();
            entity.Property(e => e.PostalCode).IsRequired();
            entity.Property(e => e.PhoneNumber).IsRequired();
        });

        modelBuilder.Entity<CraftsmanProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Bio).IsRequired();
            entity.Property(e => e.PhoneNumber).IsRequired();
            entity.Property(e => e.Address).IsRequired();
            entity.Property(e => e.City).IsRequired();
            entity.Property(e => e.Country).IsRequired();
            entity.Property(e => e.Street).IsRequired();
            entity.Property(e => e.Number).IsRequired();
            entity.Property(e => e.PostalCode).IsRequired();
            entity.Property(e => e.ProfilePicture).IsRequired();
            entity.HasOne(e => e.Craftsman)
                .WithOne(c => c.CraftsmanProfile)
                .HasForeignKey<CraftsmanProfile>(cp => cp.CraftsmanPublicId)
                .HasPrincipalKey<Craftsman>(c => c.PublicId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.CraftsmanServices)
                .WithOne(cs => cs.CraftsmanProfile)
                .HasForeignKey(cs => cs.CraftsmanProfileCraftsmanPublicId)
                .HasPrincipalKey(cp => cp.CraftsmanPublicId)
                .OnDelete(DeleteBehavior.Cascade);
        }); 

        modelBuilder.Entity<CraftsmanService>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Price).IsRequired();
            entity.Property(e => e.Duration).IsRequired();
            entity.HasOne(e => e.Service)
                .WithMany()
                .HasForeignKey(e => e.ServicePublicId)
                .HasPrincipalKey(e => e.PublicId)
                .IsRequired();
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.PublicId).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Description).IsRequired();
            entity.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryPublicId)
                .HasPrincipalKey(e => e.PublicId)
                .IsRequired();
        });
    }
}