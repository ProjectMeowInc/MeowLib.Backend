namespace MeowLib.Domain.Shared.Services;

public interface ITelegramLogService
{
    Task<Result.Result> CriticalErrorLogAsync(string from, string message);
    Task<Result.Result> ErrorLogAsync(string from, string message);
}