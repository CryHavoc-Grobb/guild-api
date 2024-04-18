using System.Text.Json.Serialization;

namespace GuildApi.UseCases.UpdateGuildBank;

public sealed class UpdateBankRequest
{    
    [JsonPropertyName("items")]
    public List<GuildBankItem> Items { get; set; } = [];

    [JsonPropertyName("fileDate")]
    public DateTimeOffset FileDate { get; set; }
}