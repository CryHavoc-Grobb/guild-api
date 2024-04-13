using GuildApi.Infrastructure.MongoDb.DataModels;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace GuildApi.Infrastructure.MongoDb;

internal static class MongoDbInstaller
{
    public const string DatabaseName = "cryhavoc";
    
    public static void AddMongoDb(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IMongoDatabase>(_ =>
        {
            var settings = MongoClientSettings.FromConnectionString(builder.Configuration["MongoDb:ConnectionString"]);
            settings.LinqProvider = LinqProvider.V3;
            var client = new MongoClient(settings);
            return client.GetDatabase(DatabaseName);
        });

        builder.Services.AddCollection<GuildBankData>(GuildBankData.CollectionName);
    }

    private static void AddCollection<T>(this IServiceCollection services, string name)
        where T : class
    {
        services.AddScoped<IMongoCollection<T>>(p =>
        {
            var db = p.GetRequiredService<IMongoDatabase>();
            return db.GetCollection<T>(name);
        });
    }

    public static async Task UseMongoDb(this WebApplication app)
    {
        var database = app.Services.GetRequiredService<IMongoDatabase>();
        
        async Task EnsureIndexes<T>()
            where T : IModelIndexProvider<T>
        {
            var indexes = T.GetIndexes();
            var collection = database.GetCollection<T>(T.CollectionName);
            await collection.Indexes.CreateManyAsync(indexes);
        }

        await EnsureIndexes<GuildBankData>();
    }
}