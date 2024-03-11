using crafts_api.configuration;
using crafts_api.context;
using crafts_api.services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// builder.Services.AddSingleton<DatabaseConfiguration>(_ =>
//     new DatabaseConfiguration(builder.Configuration));

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration["ConnectionStrings:Default"]));

// builder.Services.AddSingleton<UserService>();
builder.Services.AddScoped<CategoryService>();
// builder.Services.AddSingleton<AuthService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

