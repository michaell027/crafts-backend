using Microsoft.EntityFrameworkCore;

namespace crafts_api.context;

// dotnet ef migrations add MigrationName --context SqlDatabaseContext --output-dir Migrations/SqlServerMigrations
// dotnet ef database update --context SqlDatabaseContext
public class SqlDatabaseContext : DatabaseContext
{
    private readonly IConfiguration configuration;

        public SqlDatabaseContext(IConfiguration configuration) : base(configuration)
        {
            this.configuration = configuration;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = configuration.GetConnectionString("Default");
            optionsBuilder.UseSqlServer(connectionString);
            optionsBuilder.UseLazyLoadingProxies();
        }

}