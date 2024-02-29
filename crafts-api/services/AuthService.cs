using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using crafts_api.configuration;
using crafts_api.models;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;

namespace crafts_api.services;

public class AuthService
{
    private readonly DatabaseConfiguration _databaseConfiguration;

    private readonly IConfiguration _configuration;

    public AuthService(DatabaseConfiguration databaseConfiguration, IConfiguration configuration)
    {
        _databaseConfiguration = databaseConfiguration;
        _configuration = configuration;
    }

    // register user
    public void Register(User user)
    {
        using var connection = _databaseConfiguration.GetConnection();
        connection.Open();

        using var command =
            new MySqlCommand(
                "INSERT INTO users (username, password, email, created_at, updated_at) VALUES (@username, @password, @email, @created_at, @updated_at)",
                connection);
        command.Parameters.AddWithValue("@username", user.Username);
        command.Parameters.AddWithValue("@password", user.Password);
        command.Parameters.AddWithValue("@email", user.Email);
        command.Parameters.AddWithValue("@created_at", DateTime.Now);
        command.Parameters.AddWithValue("@updated_at", DateTime.Now);
        command.ExecuteNonQuery();

        connection.Close();
    }

    // login user
    public User Login(LoginRequest loginRequest)
    {
        using var connection = _databaseConfiguration.GetConnection();
        connection.Open();

        using var command =
            new MySqlCommand(
                "SELECT * FROM users WHERE username = @credential OR email = @credential AND password = @password",
                connection);
        command.Parameters.AddWithValue("@credential", loginRequest.Credential);
        command.Parameters.AddWithValue("@password", loginRequest.Password);

        using var reader = command.ExecuteReader();

        var loggedInUser = new User();

        while (reader.Read())
        {
            loggedInUser = new User
            {
                Id = reader.GetInt32("id"),
                Username = reader.GetString("username"),
                Password = reader.GetString("password"),
                Email = reader.GetString("email"),
                CreatedAt = reader.GetDateTime("created_at"),
                UpdatedAt = reader.GetDateTime("updated_at")
            };
        }

        connection.Close();

        return loggedInUser;
    }

    // create user token
    public string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var key = _configuration["Jwt:Key"];
        var keyBytes = Encoding.UTF8.GetBytes(key!);
        var base64EncodedKey = Convert.ToBase64String(keyBytes);
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(base64EncodedKey));

        var creds = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}