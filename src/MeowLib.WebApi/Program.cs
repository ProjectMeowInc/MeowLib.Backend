using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using MeowLib.DAL;
using MeowLib.Domain.Models;
using MeowLib.Services.Implementation.Production;
using MeowLib.Services.Interface;
using MeowLib.WebApi.Middleware;
using MeowLib.WebApi.Models.Responses.v1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var services = builder.Services;
services.AddControllers()
    .AddJsonOptions(jsonOptions =>
    {
        jsonOptions.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = actionContext =>
        {
            var validationErrors = new List<ValidationErrorModel>();

            foreach (var modelStateValue in actionContext.ModelState.Values)
            foreach (var modelError in modelStateValue.Errors)
            {
                validationErrors.Add(new ValidationErrorModel
                {
                    PropertyName = modelStateValue.RawValue?.ToString() ?? "NULL",
                    Message = modelError.ErrorMessage
                });
            }

            return new JsonResult(new ValidationErrorResponse(validationErrors))
            {
                StatusCode = 400
            };
        };
    });

services.AddEndpointsApiExplorer();

services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.ToString());
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "BEST READER API", Version = "v1" });
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
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });

    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
});

// Init services
services.AddSingleton<IHashService, HashService>();
services.AddSingleton<IUserTokenService, UserTokensService>();
services.AddSingleton<ITelegramLogService, TelegramLogService>();

var uploadFileDirectory = builder.Configuration.GetValue<string>("UploadFileDirectory");
if (string.IsNullOrEmpty(uploadFileDirectory))
{
    throw new Exception("Не указана директория для сервиса загрузки изображений");
}

services.AddScoped<IFileService>(_ => new FileService(uploadFileDirectory));

services.AddScoped<IUserService, UserService>();
services.AddScoped<IAuthorService, AuthorService>();
services.AddScoped<ITagService, TagService>();
services.AddScoped<IBookService, BookService>();
services.AddScoped<IChapterService, ChapterService>();
services.AddScoped<IUserFavoriteService, UserFavoriteService>();
services.AddScoped<IBookmarkService, BookmarkService>();
services.AddScoped<IBookCommentService, BookCommentService>();
services.AddScoped<ITeamService, TeamService>();
services.AddScoped<INotificationService, NotificationService>();
services.AddScoped<ITranslationService, TranslationService>();
services.AddScoped<ICoinService, CoinService>();
services.AddScoped<INotificationTokenService, NotificationTokenService>();

builder.Services.AddDbContext<ApplicationDbContext>(dbOptions =>
{
    var connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:SQLite");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new Exception("Connection string is null or empty");
    }

    dbOptions.UseSqlite(connectionString, optionsBuilder => { optionsBuilder.MigrationsAssembly("MeowLib.WebApi"); });
});

var app = builder.Build();

var logger = app.Services.GetService<ILogger<Program>>();
if (logger is null)
{
    throw new Exception("Логгер не инициализирован");
}

logger.LogInformation("[{@DateTime}] Начато добавление Middleware", DateTime.UtcNow);

app.UseCors(corsBuilder =>
{
    corsBuilder.AllowAnyHeader();
    corsBuilder.AllowAnyMethod();
    corsBuilder.AllowAnyOrigin();
});

app.UseMiddleware<ExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    logger.LogInformation("[{@DateTime}] Используется DEV окружение", DateTime.UtcNow);
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

logger.LogInformation("[{@DateTime}] Приложение готово к запуску", DateTime.UtcNow);

app.Run();