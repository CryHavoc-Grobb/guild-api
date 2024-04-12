using System.Text.Json.Serialization;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace GuildApi.UseCases.UpdateGuildBank;

public class Endpoint : EndpointBaseAsync
    .WithRequest<UpdateBankRequest>
    .WithActionResult
{
    [HttpPut("/bank", Name = "UPDATE_GUILD_BANK")]
    public override async Task<ActionResult> HandleAsync(
        [FromBody] UpdateBankRequest request, 
        CancellationToken cancellationToken = new())
    {
        return Ok();
    }
}

public sealed class UpdateBankRequest
{
    [JsonPropertyName("items")]
    public List<GuildBankItem> Items { get; set; } = [];
}

public sealed class GuildBankItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("count")]
    public int Count { get; set; }
}