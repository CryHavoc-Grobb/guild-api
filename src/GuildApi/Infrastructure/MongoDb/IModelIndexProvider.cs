using MongoDB.Driver;

namespace GuildApi.Infrastructure.MongoDb;

internal interface IModelIndexProvider<T>
{
    static abstract string CollectionName { get; }

    static abstract IEnumerable<CreateIndexModel<T>> GetIndexes();
}