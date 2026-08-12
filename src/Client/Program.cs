using System.Text.Json;
using System.Text.Json.Serialization;
using Void.Client;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});
builder.Services.AddSingleton<IGameRuntime, GameRuntime>();
builder.Services.AddSingleton<GameCoordinator>();
builder.Services.AddHostedService(serviceProvider => serviceProvider.GetRequiredService<GameCoordinator>());

var application = builder.Build();
application.MapClientApi();
await application.RunAsync();
