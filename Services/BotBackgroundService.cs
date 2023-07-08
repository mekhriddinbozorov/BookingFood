using Telegram.Bot;
using Telegram.Bot.Polling;

public class BotBackgroundService : BackgroundService
{
    private readonly ILogger<BotBackgroundService> logger;
    private readonly ITelegramBotClient botClient;
    private readonly IUpdateHandler updateHandler;

    public BotBackgroundService(
        ILogger<BotBackgroundService> logger,
        ITelegramBotClient botClient,
        IUpdateHandler updateHandler
    )
    {
        this.logger = logger;
        this.botClient = botClient;
        this.updateHandler = updateHandler;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var bot = await botClient.GetMeAsync(stoppingToken);

        logger.LogInformation("Bot started suffesfully : {bot.Username}", bot.Username);

        botClient.StartReceiving(
            updateHandler.HandleUpdateAsync,
            updateHandler.HandlePollingErrorAsync,
            new ReceiverOptions()
            {
                ThrowPendingUpdates = true,
            },
            stoppingToken
        );
    }
}