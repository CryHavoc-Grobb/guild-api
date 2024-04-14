using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GuildApi.Infrastructure.MongoDb.DataModels;

internal sealed class ConfigData
{
    public const string CollectionName = "config";
    
    public ObjectId Id { get; set; }

    [BsonElement("keys")] 
    public List<ApiKey> Keys { get; set; } = [];
}

internal sealed class ApiKey
{
    [BsonElement("user")]
    public string User { get; set; } = string.Empty;

    [BsonElement("key")] 
    public string Key { get; set; } = string.Empty;
}