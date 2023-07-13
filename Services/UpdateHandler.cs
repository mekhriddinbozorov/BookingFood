using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

public partial class UpdateHandler : IUpdateHandler
{
    private readonly ILogger<UpdateHandler> logger;
    private readonly IServiceScopeFactory serviceScopeFactory;

    public UpdateHandler(
        ILogger<UpdateHandler> logger,
        IServiceScopeFactory serviceScopeFactory
    )
    {
        this.logger = logger;
        this.serviceScopeFactory = serviceScopeFactory;
    }
    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Error while Bot polling.");
        return Task.CompletedTask;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            await HandleMessageUpdateAsync(botClient, update, cancellationToken);
        }
        catch (Exception ex)
        {
            await HandlePollingErrorAsync(botClient, ex, cancellationToken);
        }
    }

    private Task HandleUnknownUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received {updateType} update.", update.Type);

        return Task.CompletedTask;
    }
}