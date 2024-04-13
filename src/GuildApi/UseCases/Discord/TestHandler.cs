using Discord.Interactions;

namespace GuildApi.UseCases.Discord;

public class TestHandler : InteractionModuleBase
{
    [SlashCommand("foo", "This does a foo")]
    public async Task Echo(string input)
    {
        await RespondAsync("Yeah bub!", ephemeral: true);
    }
}