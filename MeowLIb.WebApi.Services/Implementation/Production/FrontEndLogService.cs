using System.Text;
using MeowLib.Domain.Models;
using MeowLIb.WebApi.Services.Interface;
using Telegram.Bot;

namespace MeowLIb.WebApi.Services.Implementation.Production;

public class FrontEndLogService : IFrontEndLogService
{
    private readonly TelegramBotClient _botClient;
    private readonly long _chatId;

    public FrontEndLogService()
    {
        // change to cfg
        var botToken = "0";
        var chatId = -1001781472659;

        _botClient = new TelegramBotClient(botToken);

        _chatId = chatId;
    }

    public async Task LogAsync(string userLogin, ErrorLogInfoModel errorData)
    {
        var additionalInfoStringBuilder = new StringBuilder();

        if (errorData.AdditionalInfo is not null && errorData.AdditionalInfo.Any())
        {
            additionalInfoStringBuilder.AppendLine("🦄 Дополнительная информация:\n");
            foreach (var (param, message) in errorData.AdditionalInfo)
            {
                additionalInfoStringBuilder.AppendLine($"- {param}: {message}");
            }
        }

        var isApiError = errorData.IsApiError ? "Да" : "Нет";
        var textToSend = "[Лог | FrontEnd]\n\n" +
                         $"📅 Дата: {DateTime.Now}\n\n" +
                         $"✉️ Сообщение: {errorData.Message}\n\n" +
                         $"📦 Модуль в котором произошла ошибка: {errorData.ErrorModule}\n\n" +
                         $"👤 Логин пользователя: {userLogin}\n\n" +
                         $"🕷️ Ошибка связана c Api: {isApiError}\n\n" +
                         $"{additionalInfoStringBuilder}";

        await _botClient.SendTextMessageAsync(_chatId, textToSend);
    }
}