using System.Text.Json.Serialization;

namespace GuildApi.UseCases.UpdateGuildBank;

public sealed class GuildBankItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("count")]
    public int Count { get; set; }
}