using crafts_api.context;
using crafts_api.interfaces;
using crafts_api.Interfaces;
using crafts_api.Problems;
using crafts_api.services;
using crafts_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using crafts_api;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddHttpClient();
// multiple database contexts
var environment = builder.Environment;

if (environment.IsDevelopment())
{
    builder.Services.AddDbContext<SqlDatabaseContext>();
    builder.Services.AddScoped<DatabaseContext, SqlDatabaseContext>();
}
else
{
    builder.Services.AddDbContext<DatabaseContext>();
}

// builder.Services.AddDbContext<DatabaseContext>((provider, options) =>
// {
//     var webHostEnvironment = provider.GetRequiredService<IWebHostEnvironment>();
//     var isDevelopment = webHostEnvironment.IsDevelopment();
//     
//     var connectionString = config.GetConnectionString("Default");
//     
//     if (isDevelopment)
//     {
//         options.UseSqlServer(connectionString);
//         options.UseLazyLoadingProxies();
//     }
//     else
//     {
//         options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
//         options.UseLazyLoadingProxies();
//     }
// });

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<ICraftersService, CraftersService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddLogging();
 
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Crafts API",
        Description = "A simple API to manage crafts",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Michaela Majoro�ov�",
            Email = "miselka12345@gmail.com"
        },
        License = new OpenApiLicense
        {
            Name = "Use under LICX",
            Url = new Uri("https://example.com/license")
        }
    });
    // var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    // options.IncludeXmlComments(xmlPath);
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<DatabaseContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateActor = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        RequireExpirationTime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    
    using (var serviceScope = app.Services.GetService<IServiceScopeFactory>()!.CreateScope())
    {
        var context = serviceScope.ServiceProvider.GetRequiredService<SqlDatabaseContext>();
        context.Database.EnsureCreated();
    }
}

app.UseMiddleware<CustomErrorMiddleware>();

app.InitializeDatabase().GetAwaiter();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

