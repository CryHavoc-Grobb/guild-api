using Ardalis.ApiEndpoints;
using GuildApi.Infrastructure.MongoDb.DataModels;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace GuildApi.UseCases.UpdateGuildBank;

public class Endpoint(IServiceProvider services) : EndpointBaseAsync
    .WithRequest<UpdateBankRequest>
    .WithActionResult
{
    private IMongoCollection<GuildBankData> _guildBank
        = services.GetRequiredService<IMongoCollection<GuildBankData>>();
    
    [HttpPut("/bank", Name = "UPDATE_GUILD_BANK")]
    public override async Task<ActionResult> HandleAsync(
        [FromBody] UpdateBankRequest request, 
        CancellationToken cancellationToken = new())
    {
        var lastUpdate = DateTimeOffset.UtcNow;
        var newData = request.Items
            .GroupBy(i => i.Id)
            .Select(g => new GuildBankData
            {
                ItemId = g.Key,
                Name = g.First().Name,
                Quantity = g.Sum(i => i.Count),
                LastUpdate = lastUpdate
            });
        
        // truncate the guild bank
        await _guildBank.DeleteManyAsync(
            FilterDefinition<GuildBankData>.Empty,
            cancellationToken);
        
        // reload
        await _guildBank.InsertManyAsync(newData, cancellationToken: cancellationToken);

        return Ok();
    }
}