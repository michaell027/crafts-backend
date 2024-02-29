using crafts_api.configuration;
using crafts_api.models;
using MySqlConnector;

namespace crafts_api.services;

public class CategoryService
{
    private readonly DatabaseConfiguration _databaseConfiguration;
    
    public CategoryService(DatabaseConfiguration databaseConfiguration) =>
        _databaseConfiguration = databaseConfiguration;
    
    // get all categories
    public List<Category> GetAllCategories()
    {
        using var connection = _databaseConfiguration.GetConnection();
        connection.Open();

        using var command = new MySqlCommand("SELECT * FROM categories", connection);
        using var reader = command.ExecuteReader();
        
        var categories = new List<Category>();

        while (reader.Read())
        {
            categories.Add(new Category
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                SkName = reader.GetString("sk_name")
            });
        }

        connection.Close();
        
        return categories;
    }
    
}