using crafts_api.context;
using crafts_api.exceptions;
using crafts_api.interfaces;
using crafts_api.models.domain;
using crafts_api.models.models;
using crafts_api.Utils;
using System.Net;

namespace crafts_api.services;

public class AuthService : IAuthService
{
    private readonly DatabaseContext _databaseContext;
    private readonly PasswordFunctions passwordFunctions;

    public AuthService(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
        passwordFunctions = new PasswordFunctions();
    }

    public Task<User> Login(LoginRequest loginRequest)
    {
        throw new NotImplementedException();
    }

    public async Task Register(RegisterRequest registerRequest)
    {
        Boolean emailExists = _databaseContext.Users.Any(user => user.Email == registerRequest.Email);
        if (emailExists)
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "Email already exists"
            };
        }

        Boolean passwordsMatch = registerRequest.Password == registerRequest.PasswordConfirmation;
        if (!passwordsMatch)
        {
            throw new DefaultException
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = 400,
                Message = "Passwords do not match"
            };
        }

        User user = new User
        {
            PublicId = Guid.NewGuid(),
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName,
            Password = passwordFunctions.HashPassword(registerRequest.Password),
            Email = registerRequest.Email,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await _databaseContext.Users.AddAsync(user);
        await _databaseContext.SaveChangesAsync();
    }

    public void TestError()
    {
        throw new DefaultException
        {
            StatusCode = HttpStatusCode.InternalServerError,
            ErrorCode = 500,
            Message = "Test error"
        };
    }
}