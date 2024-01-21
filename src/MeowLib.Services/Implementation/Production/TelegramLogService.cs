using MeowLib.Domain.Shared;
using MeowLib.Domain.Shared.Result;
using MeowLib.Domain.Shared.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace MeowLib.Services.Implementation.Production;

public class TelegramLogService : ITelegramLogService
{
    private readonly TelegramBotClient _botClient;
    private readonly long _chatId;
    private readonly ILogger<TelegramLogService> _logger;

    public TelegramLogService(ILogger<TelegramLogService> logger)
    {
        _logger = logger;
        // change to cfg
        var botToken = "0";
        var chatId = -1001781472659;

        _botClient = new TelegramBotClient(botToken);

        _chatId = chatId;
    }

    public Task<Result> CriticalErrorLogAsync(string from, string message)
    {
        return SendLogAsync("Critical", from, message);
    }

    public Task<Result> ErrorLogAsync(string from, string message)
    {
        return SendLogAsync("Error", from, message);
    }

    private async Task<Result> SendLogAsync(string type, string from, string message)
    {
        var textToSend = $"[{type}Log]\n\n" +
                         $"📅 Дата: {DateTime.Now}\n\n" +
                         $"✉️ Сообщение: {message}\n\n" +
                         $"📦 Модуль в котором произошла ошибка: {from}";
        try
        {
            await _botClient.SendTextMessageAsync(_chatId, textToSend);
        }
        catch (Exception e)
        {
            _logger.LogError("Error send telegram log: {errorMessage}", e.Message);
            return Result.Fail(new InnerException($"Ошибка отправки логов в телеграм: {e.Message}"));
        }

        return Result.Ok();
    }
}