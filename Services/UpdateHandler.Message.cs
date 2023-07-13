using BookingFood.Data;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

public partial class UpdateHandler
{
    private readonly Dictionary<long, int> deleteRequestMessages = new Dictionary<long, int>();
    private async Task HandleMessageUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var username = update.Message.From?.Username ?? update.Message.From.FirstName;
        logger.LogInformation("Received Message from {username}", username);

        if (deleteRequestMessages.ContainsKey(update.Message.Chat.Id))
        {
            await RemoveMessageAsync(botClient, update.Message.Chat.Id, deleteRequestMessages[update.Message.Chat.Id]);
            deleteRequestMessages.Remove(update.Message.Chat.Id);
        }

        if (update.Message.Text == "/start" || update.Message.Text == "/home")
        {
            var telegramUser = GetUserFromUpdate(update);

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == telegramUser.Id);

                if (user is null)
                {
                    dbContext.Users.Add(new BookingFood.Entities.User
                    {
                        Id = telegramUser.Id,
                        Fullname = $"{telegramUser.FirstName} {telegramUser.LastName}",
                        Username = telegramUser.Username,
                        Language = telegramUser.LanguageCode,
                        CreatedAt = DateTime.UtcNow,
                        ModifiedAt = DateTime.UtcNow
                    });
                    logger.LogInformation("New user with ID {id} added.", telegramUser.Id);
                }
                else
                {
                    user.Fullname = $"{telegramUser.FirstName} {telegramUser.LastName}";
                    user.Username = telegramUser.Username;
                    user.Language = telegramUser.LanguageCode;
                    user.CreatedAt = DateTime.UtcNow;
                    user.ModifiedAt = DateTime.UtcNow;
                    logger.LogInformation("New user with ID {id} updated.", telegramUser.Id);
                }
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            var replyKeyboardMarkup = new ReplyKeyboardMarkup(
                    new KeyboardButton[][]
                    {
                        new KeyboardButton[] { "Settings", "Menu" },
                        new KeyboardButton[] { "Others" },
                    }
                );

            var sendMessage = await botClient.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: $"Welcome {telegramUser.Username}, This is Booking Food telegram bot. 👋",
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken
            );

        }
        else if (update.Message.Text == "Settings")
        {
            var replayLangandLocation = new ReplyKeyboardMarkup(
                new KeyboardButton[][]
                {
                        new KeyboardButton[] { "Language", "Location" }
                }
            );

            var sendMessageSettings = await botClient.SendTextMessageAsync(
            chatId: update.Message.Chat.Id,
            text: "",
            replyMarkup: replayLangandLocation,
            cancellationToken: cancellationToken
            );
        }
        else
        {
            var sendMessage = await botClient.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: "You can start from /start or /home !",
                cancellationToken: cancellationToken
            );
        }
    }

    private User GetUserFromUpdate(Update update)
        => update.Type switch
        {
            Telegram.Bot.Types.Enums.UpdateType.Message => update.Message.From,
            Telegram.Bot.Types.Enums.UpdateType.EditedMessage => update.EditedMessage.From,
            Telegram.Bot.Types.Enums.UpdateType.CallbackQuery => update.CallbackQuery.From,
            Telegram.Bot.Types.Enums.UpdateType.InlineQuery => update.InlineQuery.From,
            _ => throw new Exception("We dont supportas update type {update.Type} yet")
        };

    private async Task RemoveMessageAsync(
       ITelegramBotClient botClient,
       long chatId, int messageId,
       TimeSpan delay = default,
       CancellationToken cancellationToken = default)
    {
        await Task.Delay(delay, cancellationToken);
        await botClient.DeleteMessageAsync(chatId, messageId, cancellationToken);
    }
}