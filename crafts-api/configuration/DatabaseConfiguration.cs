using MySqlConnector;

namespace crafts_api.configuration
{
    public class DatabaseConfiguration
    {
        private readonly IConfiguration _configuration;

        public DatabaseConfiguration(IConfiguration configuration) => _configuration = configuration;

        public MySqlConnection GetConnection() => new MySqlConnection(_configuration.GetConnectionString("SQLCONNSTR"));
    }
}