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