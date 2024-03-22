//using crafts_api.configuration;
//using crafts_api.models;
//using MySqlConnector;

//namespace crafts_api.services;

//public class UserService
//{
//    private readonly DatabaseConfiguration _databaseConfiguration;
    
//    public UserService(DatabaseConfiguration databaseConfiguration) =>
//        _databaseConfiguration = databaseConfiguration;

//    // get all users
//    public List<User> GetAllUsers()
//    {
//        using var connection = _databaseConfiguration.GetConnection();
//        connection.Open();

//        using var command = new MySqlCommand("SELECT * FROM users", connection);
//        using var reader = command.ExecuteReader();
        
//        var users = new List<User>();

//        while (reader.Read())
//        {
//            users.Add(new User
//            {
//                Id = reader.GetInt32("id"),
//                Username = reader.GetString("username"),
//                Password = reader.GetString("password"),
//                Email = reader.GetString("email"),
//                CreatedAt = reader.GetDateTime("created_at"),
//                UpdatedAt = reader.GetDateTime("updated_at")
//            });
//        }

//        connection.Close();
        
//        return users;
//    }
    
//    // get user by id
//    public User GetUserById(int id)
//    {
//        using var connection = _databaseConfiguration.GetConnection();
//        connection.Open();

//        using var command = new MySqlCommand("SELECT * FROM users WHERE id = @id", connection);
//        command.Parameters.AddWithValue("@id", id);
//        using var reader = command.ExecuteReader();
        
//        var user = new User();

//        while (reader.Read())
//        {
//            user = new User
//            {
//                Id = reader.GetInt32("id"),
//                Username = reader.GetString("username"),
//                Password = reader.GetString("password"),
//                Email = reader.GetString("email"),
//                CreatedAt = reader.GetDateTime("created_at"),
//                UpdatedAt = reader.GetDateTime("updated_at")
//            };
//        }

//        connection.Close();
        
//        return user;
//    }
//}