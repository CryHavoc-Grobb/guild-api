using System.Runtime.CompilerServices;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using GuildApi.UseCases.QueryGuildBank;

namespace GuildApi.UseCases.Discord;

internal sealed class DiscordListener(
    IServiceProvider services, 
    ILogger<DiscordListener> logger) : BackgroundService
{
    private readonly IServiceProvider _services = services.CreateScope().ServiceProvider;
    private DiscordSocketClient _client = default!;
    private ILogger _logger;
    private ulong _guildId;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = _services.GetRequiredService<IConfiguration>();
        _client = _services.GetRequiredService<DiscordSocketClient>();
        _logger = _services.GetRequiredService<ILogger<DiscordListener>>();
        _guildId = config.GetValue<ulong>("GuildId");

        _client.Ready += OnClientReady;
        _client.SlashCommandExecuted += SlashHandler;
        _client.Log += message => 
        { 
            logger.LogDebug("{Message}", message.Message);
            return Task.CompletedTask;
        };

        logger.LogInformation("Starting discord bot");
        
        var token = config["Discord:Token"];
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task OnClientReady()
    {
        var guild = _client.GetGuild(_guildId);
        foreach (var command in GetSlashCommands())
        {
            await guild.CreateApplicationCommandAsync(command);    
        }
    }

    private async Task SlashHandler(SocketSlashCommand command)
    {
        switch (command.CommandName)
        {
            case "gbank":
                await GbankSearch(command);
                break;
            default:
                await command.RespondAsync("Unknown command");
                break;
        }
    }

    private static ICollection<SlashCommandProperties> GetSlashCommands()
    {
        var bankCommandBuilder = new SlashCommandBuilder()
            .WithName("gbank")
            .WithDescription("Query the guild bank for an item")
            .AddOption("query", 
                ApplicationCommandOptionType.String, 
                "The item you're searching for", 
                isRequired: true);
        
        var bankCommand = bankCommandBuilder.Build();

        return [bankCommand];
    }
    
    private async Task GbankSearch(SocketSlashCommand command)
    {
        try
        {
            var searchService = _services
                .CreateScope()
                .ServiceProvider
                .GetRequiredService<GuildBankQuery>();

            var queryTerm = (string)command.Data.Options.First().Value;
            var (result, lastUpdate) = await searchService.SearchBank(queryTerm, default);

            var embedBuilder = new EmbedBuilder()
                .WithAuthor("Cry Havoc Bank")
                .WithTitle("Bank Search Results")
                .WithDescription(result)
                .WithColor(Color.Green)
                .WithFooter($"Last Update: {lastUpdate:MMM dd} at {lastUpdate:t} GMT");

            await command.RespondAsync(embed: embedBuilder.Build(), ephemeral: true);
        }
        catch (Exception ex)
        {
            var embedBuilder = new EmbedBuilder()
                .WithAuthor("Cry Havoc Bank")
                .WithTitle("Bank Search Results")
                .WithDescription("There was a server error. Bug Sin about it.")
                .WithColor(Color.Red);

            await command.RespondAsync(embed: embedBuilder.Build(), ephemeral: true);
            _logger.LogCritical(ex, "{Error}", ex.Message);
        }
    }
}