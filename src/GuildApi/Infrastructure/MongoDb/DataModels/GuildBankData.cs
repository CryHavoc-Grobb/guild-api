using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace GuildApi.Infrastructure.MongoDb.DataModels;

internal sealed class GuildBankData : IModelIndexProvider<GuildBankData>
{
    public static string CollectionName => "guildBank";
    
    public ObjectId Id { get; set; }

    [BsonElement("item_id")]
    public int ItemId { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("quantity")]
    public int Quantity { get; set; }
    
    [BsonElement("last_update")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset LastUpdate { get; set; }
    
    public static IEnumerable<CreateIndexModel<GuildBankData>> GetIndexes()
    {
        var nameIdx = new CreateIndexModel<GuildBankData>(
            Builders<GuildBankData>.IndexKeys
                .Text(i => i.Name));

        return [nameIdx];
    }
}