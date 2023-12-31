using MeowLib.Domain.Shared.Result;

namespace MeowLib.Domain.Team.Services;

public interface ITelegramLogService
{
    Task<Result> CriticalErrorLogAsync(string from, string message);
    Task<Result> ErrorLogAsync(string from, string message);
}