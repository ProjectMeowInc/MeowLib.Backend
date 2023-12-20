using MeowLib.Domain.Result;

namespace MeowLib.Services.Interface;

public interface ITelegramLogService
{
    Task<Result> CriticalErrorLogAsync(string from, string message);
    Task<Result> ErrorLogAsync(string from, string message);
}