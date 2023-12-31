using MeowLib.WebApi.Extensions;
using MeowLib.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var services = builder.Services;
services.InitControllers();

services.AddEndpointsApiExplorer();

services.InitSwaggerDocs();

services.InitServices(builder);

services.InitDatabase(builder);

var app = builder.Build();

var logger = app.Services.GetService<ILogger<Program>>();
if (logger is null)
{
    throw new Exception("Логгер не инициализирован");
}

logger.LogInformation("[{@DateTime}] Начато добавление Middleware", DateTime.UtcNow);

app.SetUpCors();

app.UseMiddleware<ExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    logger.LogInformation("[{@DateTime}] Используется DEV окружение", DateTime.UtcNow);
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}

app.MapControllers();

logger.LogInformation("[{@DateTime}] Приложение готово к запуску", DateTime.UtcNow);

app.Run();