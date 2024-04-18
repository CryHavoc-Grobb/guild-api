using Ardalis.ApiEndpoints;
using GuildApi.Infrastructure.MongoDb.DataModels;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace GuildApi.UseCases.UpdateGuildBank;

public class Endpoint(
    IServiceProvider services,
    ILogger<Endpoint> logger) : EndpointBaseAsync
        .WithRequest<UpdateBankRequest>
        .WithActionResult
{
    private readonly IMongoCollection<GuildBankData> _guildBank
        = services.GetRequiredService<IMongoCollection<GuildBankData>>();

    private readonly IMongoCollection<ConfigData> _config
        = services.GetRequiredService<IMongoCollection<ConfigData>>();
    
    [HttpPut("/bank", Name = "UPDATE_GUILD_BANK")]
    public override async Task<ActionResult> HandleAsync(
        [FromBody] UpdateBankRequest request, 
        CancellationToken cancellationToken = new())
    {
        var isAuthorized = await IsAuthorized(cancellationToken);
        if (!isAuthorized)
            return Unauthorized();

        var firstRecord = _guildBank.AsQueryable().First();
        if (request.FileDate <= firstRecord.LastUpdate)
        {
            logger.LogInformation("Older file was ignored. {Incoming} vs. {Latest}", 
                request.FileDate, 
                firstRecord.LastUpdate);
            
            return Ok();
        }
        
        var newData = request.Items
            .GroupBy(i => i.Id)
            .Select(g => new GuildBankData
            {
                ItemId = g.Key,
                Name = g.First().Name,
                Quantity = g.Sum(i => i.Count),
                LastUpdate = request.FileDate
            });
        
        // truncate the guild bank
        await _guildBank.DeleteManyAsync(
            FilterDefinition<GuildBankData>.Empty,
            cancellationToken);
        
        // reload
        await _guildBank.InsertManyAsync(newData, cancellationToken: cancellationToken);

        return Ok();
    }

    private async Task<bool> IsAuthorized(CancellationToken cancellationToken)
    {
        // Lazy implementation - move this to auth framework
        var hasAuth = Request.Headers.TryGetValue("Authorization", out var auth);
        if (!hasAuth || auth.Count < 1)
            return false;

        var configCursor = await _config.FindAsync(
            FilterDefinition<ConfigData>.Empty, 
            cancellationToken: cancellationToken);

        var config = await configCursor.FirstAsync(cancellationToken);
        var token = auth.First()!.Replace("token ", string.Empty);
        var user = config.Keys.SingleOrDefault(k => k.Key == token);
        if (user is null)
            return false;
        
        logger.LogInformation("API call by {User}", user.User);
        return true;
    }
}