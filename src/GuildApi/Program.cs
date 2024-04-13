using FluentValidation;
using GuildApi.Infrastructure.MongoDb;
using GuildApi.UseCases.Discord;
using GuildApi.UseCases.QueryGuildBank;

var builder = WebApplication.CreateBuilder(args);

builder.AddDiscord();
builder.AddMongoDb();
builder.Services.AddScoped<GuildBankQuery>();

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddControllers();

builder.Services.AddHostedService<DiscordListener>();

var app = builder.Build();
await app.UseMongoDb();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
