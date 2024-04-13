using FluentValidation;
using GuildApi.Infrastructure.MongoDb;

var builder = WebApplication.CreateBuilder(args);

builder.AddMongoDb();

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddControllers();

var app = builder.Build();
await app.UseMongoDb();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
