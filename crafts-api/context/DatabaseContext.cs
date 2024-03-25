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
    public DbSet<Crafter> Crafters { get; set; }

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
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.Role).IsRequired().HasConversion<string>();
            entity.HasIndex(e => e.IdentityId).IsUnique();
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

        modelBuilder.Entity<Crafter>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.PublicId).IsRequired();
            entity.Property(e => e.UserPublicId).IsRequired();
            entity.Property(e => e.CategoryPublicId).IsRequired();
            entity.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserPublicId)
            .HasPrincipalKey(e => e.PublicId)
            .IsRequired();
            entity.HasOne(e => e.Category)
            .WithMany()
            .HasForeignKey(e => e.CategoryPublicId)
            .HasPrincipalKey(e => e.PublicId)
            .IsRequired();
        });
    }
}