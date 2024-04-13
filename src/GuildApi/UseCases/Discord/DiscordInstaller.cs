using Discord.Interactions;
using Discord.WebSocket;

namespace GuildApi.UseCases.Discord;

internal static class DiscordInstaller
{
    public static void AddDiscord(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<DiscordSocketClient>();
        builder.Services.AddSingleton<InteractionService>();
    }
}