using System.Text.Encodings.Web;
using System.Text.Unicode;
using MeowLib.Domain.MappingProfiles;
using MeowLib.WebApi.DAL;
using MeowLib.WebApi.DAL.Repository.Implementation.Production;
using MeowLib.WebApi.DAL.Repository.Interfaces;
using MeowLib.WebApi.Middleware;
using MeowLIb.WebApi.Services.Implementation.Production;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddControllers()
    .AddJsonOptions(jsonOptions =>
    {
        jsonOptions.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
    });
    
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddAutoMapper(typeof(MappingProfile));

// Init repos
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IAuthorRepository, AuthorRepository>();

// Init services
services.AddSingleton<IHashService, HashService>();
services.AddSingleton<IJwtTokenService, JwtTokensService>();

services.AddScoped<IUserService, UserService>();
services.AddScoped<IAuthorService, AuthorService>();

builder.Services.AddDbContext<ApplicationDbContext>(dbOptions =>
{
    var connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:Main");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new Exception("Connection string is null or empty");
    }

    dbOptions.UseNpgsql(connectionString);
});

var app = builder.Build();

app.UseCors(corsBuilder =>
{
    corsBuilder.AllowAnyHeader();
    corsBuilder.AllowAnyMethod();
    corsBuilder.AllowAnyOrigin();
});

app.UseMiddleware<ExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();