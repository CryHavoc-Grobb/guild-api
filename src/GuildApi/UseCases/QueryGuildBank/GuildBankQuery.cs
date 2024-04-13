using System.Text;
using GuildApi.Infrastructure.MongoDb.DataModels;
using MongoDB.Driver;

namespace GuildApi.UseCases.QueryGuildBank;

internal sealed class GuildBankQuery(IMongoCollection<GuildBankData> guildBank)
{
    public async Task<(string Result, DateTimeOffset LastUpdate)> SearchBank(string query, CancellationToken cancellation)
    {
        var match = Builders<GuildBankData>.Filter.Text(query);
        var options = new FindOptions<GuildBankData, GuildBankData>
        {
            Limit = 5,
            Skip = 0
        };
        
        var cursor  = await guildBank.FindAsync(match, options, cancellationToken: cancellation);
        var results = await cursor.ToListAsync(cancellation);

        var latestUpdate = results.Count > 0 ? results[0].LastUpdate : DateTimeOffset.UtcNow;
        if (results.Count < 1)
        {
            return ("No Results!", latestUpdate);
        }
        
        var sb = new StringBuilder();
        for (var i = 0; i < results.Count; i++)
        {
            var result = results[i];
            sb.AppendLine($"{i + 1}. {result.Name} ({result.Quantity})");
        }

        return (sb.ToString(), latestUpdate);
    }
}