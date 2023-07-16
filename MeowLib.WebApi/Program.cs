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
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddControllers()
    .AddJsonOptions(jsonOptions =>
    {
        jsonOptions.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
    });
    
services.AddEndpointsApiExplorer();

services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "BEST READER API", Version = "v1"});
    options.AddSecurityDefinition("AuthToken", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "AuthToken",
        Description = "Access token",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "AuthToken",
                    Type = ReferenceType.SecurityScheme,
                }
            },
            new List<string>()
        }
    });
});

services.AddAutoMapper(typeof(MappingProfile));

// Init repos
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IAuthorRepository, AuthorRepository>();
services.AddScoped<ITagRepository, TagRepository>();
services.AddScoped<IBookRepository, BookRepository>();
services.AddScoped<IChapterRepository, ChapterRepository>();
services.AddScoped<IUserFavoriteRepository, UserFavoriteRepository>();
services.AddScoped<IBookmarkRepository, BookmarkRepository>();

// Init services
services.AddSingleton<IHashService, HashService>();
services.AddSingleton<IJwtTokenService, JwtTokensService>();
services.AddSingleton<IFrontEndLogService, FrontEndLogService>();

var uploadFileDirectory = builder.Configuration.GetValue<string>("UploadFileDirectory");
if (string.IsNullOrEmpty(uploadFileDirectory))
{
    throw new Exception("Не указан токен для сервиса загрузки изображений");
}

services.AddScoped<IFileService>(_ => new FileService(uploadFileDirectory));

services.AddScoped<IUserService, UserService>();
services.AddScoped<IAuthorService, AuthorService>();
services.AddScoped<ITagService, TagService>();
services.AddScoped<IBookService, BookService>();
services.AddScoped<IChapterService, ChapterService>();
services.AddScoped<IUserFavoriteService, UserFavoriteService>();
services.AddScoped<IBookmarkService, BookmarkService>();

builder.Services.AddDbContext<ApplicationDbContext>(dbOptions =>
{
    var connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:SQLite");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new Exception("Connection string is null or empty");
    }

    dbOptions.UseSqlite(connectionString, optionsBuilder =>
    {
        optionsBuilder.MigrationsAssembly("MeowLib.WebApi");
    });
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