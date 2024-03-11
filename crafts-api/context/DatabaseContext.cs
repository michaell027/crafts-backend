using crafts_api.models.domain;
using Microsoft.EntityFrameworkCore;

namespace crafts_api.context;

public class DatabaseContext:DbContext
{
    private IConfiguration _configuration;
    
    public DatabaseContext(IConfiguration configuration) => _configuration = configuration;
    
    public DbSet<Category> Categories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _configuration.GetConnectionString("Default");
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
}