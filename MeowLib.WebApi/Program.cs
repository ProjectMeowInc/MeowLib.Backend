using MeowLib.Domain.MappingProfiles;
using MeowLib.WebApi.DAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddDbContext<ApplicationDbContext>(dbOptions =>
{
    var connectionString = builder.Configuration.GetValue<string>("ConnectionString:Main");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new Exception("Connection string is null or empty");
    }

    dbOptions.UseNpgsql(connectionString);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();