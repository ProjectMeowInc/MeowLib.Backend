using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using MeowLib.DAL;
using MeowLib.Domain.Author.Services;
using MeowLib.Domain.Book.Services;
using MeowLib.Domain.BookComment.Services;
using MeowLib.Domain.Bookmark.Services;
using MeowLib.Domain.Chapter.Services;
using MeowLib.Domain.CoinsChangeLog.Services;
using MeowLib.Domain.File.Services;
using MeowLib.Domain.Notification.Services;
using MeowLib.Domain.Shared.Services;
using MeowLib.Domain.Tag.Services;
using MeowLib.Domain.Team.Services;
using MeowLib.Domain.Translation.Services;
using MeowLib.Domain.User.Services;
using MeowLib.Domain.UserFavorite.Services;
using MeowLib.Services.Implementation.Production;
using MeowLib.WebApi.Models.Responses.v1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace MeowLib.WebApi.Extensions;

public static class ServicesExtensions
{
    public static void InitControllers(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddControllers()
            .AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = _ =>
                    new JsonResult(new BaseErrorResponse("Ошибка валидации данных"))
                    {
                        StatusCode = 400
                    };
            });
    }

    public static void InitSwaggerDocs(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSwaggerGen(options =>
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
    }

    public static void InitServices(this IServiceCollection serviceCollection, WebApplicationBuilder builder)
    {
        // Init services
        serviceCollection.AddSingleton<IHashService, HashService>();
        serviceCollection.AddSingleton<IUserTokenService, UserTokensService>();
        serviceCollection.AddSingleton<ITelegramLogService, TelegramLogService>();

        var uploadFileDirectory = builder.Configuration.GetValue<string>("UploadFileDirectory");
        if (string.IsNullOrEmpty(uploadFileDirectory))
        {
            throw new Exception("Не указана директория для сервиса загрузки изображений");
        }

        serviceCollection.AddScoped<IFileService, FileService>();
        serviceCollection.AddScoped<IUserService, UserService>();
        serviceCollection.AddScoped<IPeopleService, PeopleService>();
        serviceCollection.AddScoped<ITagService, TagService>();
        serviceCollection.AddScoped<IBookService, BookService>();
        serviceCollection.AddScoped<IChapterService, ChapterService>();
        serviceCollection.AddScoped<IUserFavoriteService, UserFavoriteService>();
        serviceCollection.AddScoped<IBookmarkService, BookmarkService>();
        serviceCollection.AddScoped<IBookCommentService, BookCommentService>();
        serviceCollection.AddScoped<ITeamService, TeamService>();
        serviceCollection.AddScoped<INotificationService, NotificationService>();
        serviceCollection.AddScoped<ITranslationService, TranslationService>();
        serviceCollection.AddScoped<ICoinService, CoinService>();
        serviceCollection.AddScoped<INotificationTokenService, NotificationTokenService>();
    }

    public static void InitDatabase(this IServiceCollection serviceCollection, WebApplicationBuilder builder)
    {
        serviceCollection.AddDbContext<ApplicationDbContext>(dbOptions =>
        {
            var connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:SQLite");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Connection string is null or empty");
            }

            dbOptions.UseSqlite(connectionString,
                optionsBuilder => { optionsBuilder.MigrationsAssembly("MeowLib.WebApi"); });
        });
    }
}