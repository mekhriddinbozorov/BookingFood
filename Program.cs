using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Polling;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppdbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgressConnection")));
    
builder.Services.AddHostedService<BotBackgroundService>();

builder.Services.AddSingleton<ITelegramBotClient>(
    new TelegramBotClient(builder.Configuration.GetValue("BotApiKey", string.Empty)));

builder.Services.AddTransient<IUpdateHandler, UpdateHandler>();

var app = builder.Build();

app.Run();
