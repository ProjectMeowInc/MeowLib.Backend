namespace MeowLib.WebApi.Extensions;

public static class AppExtensions
{
    public static void SetUpCors(this WebApplication application)
    {
        application.UseCors(corsBuilder =>
        {
            corsBuilder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin();
        });
    }
}