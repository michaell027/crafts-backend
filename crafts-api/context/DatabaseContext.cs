using crafts_api.Entities.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace crafts_api.context;

// dotnet ef migrations add MigrationName --context DatabaseContext --output-dir Migrations/MySqlMigrations
// dotnet ef database update --context DatabaseContext

public class DatabaseContext(IConfiguration configuration) : IdentityDbContext
{
    
    // public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    // {
    // }
    
    public DbSet<Category> Categories { get; init; } = null!;
    public new DbSet<User> Users { get; init; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; init; } = null!;
    public DbSet<Craftsman> Crafters { get; init; } = null!;
    public DbSet<UserProfile> UserProfiles { get; init; } = null!;
    public DbSet<CraftsmanProfile> CraftsmanProfiles { get; init; } = null!;
    public DbSet<CraftsmanService> CraftsmanServices { get; init; } = null!;
    public DbSet<Service> Services { get; init; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = configuration.GetConnectionString("Default");
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        optionsBuilder.UseLazyLoadingProxies();
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
            entity.Property(e => e.ProfilePicture);
            entity.Property(e => e.Country).IsRequired();
            entity.Property(e => e.City).IsRequired();
            entity.Property(e => e.Address).IsRequired();
            entity.Property(e => e.Street).IsRequired();
            entity.Property(e => e.Number).IsRequired();
            entity.Property(e => e.PostalCode).IsRequired();
            entity.Property(e => e.PhoneNumber).IsRequired();
            entity.HasOne(e => e.User)
                .WithOne(u => u.UserProfile)
                .HasForeignKey<UserProfile>(up => up.UserPublicId)
                .HasPrincipalKey<User>(u => u.PublicId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CraftsmanProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Bio);
            entity.Property(e => e.PhoneNumber).IsRequired();
            entity.Property(e => e.Address);
            entity.Property(e => e.City).IsRequired();
            entity.Property(e => e.Country).IsRequired();
            entity.Property(e => e.Street).IsRequired();
            entity.Property(e => e.Number).IsRequired();
            entity.Property(e => e.PostalCode).IsRequired();
            entity.Property(e => e.ProfilePicture);
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
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
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