using IdentificationHost.Services;
using IdentificationInfrastructure.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

string postgresConnection = builder.Configuration.GetConnectionString("Postgres")
                            ?? "Host=localhost;Database=chatmf;Username=user;Password=pass";

string redisConnection = builder.Configuration.GetConnectionString("Redis")
                         ?? "localhost:6379";

int tokenTtlSeconds = builder.Configuration.GetValue<int>("TokenTtlSeconds", 3600);

builder.Services.AddGrpc();

builder.Services.AddIdentificationInfrastructure(
    postgresConnection,
    redisConnection,
    TimeSpan.FromSeconds(tokenTtlSeconds));

WebApplication app = builder.Build();

app.MapGrpcService<AuthGrpcService>();

app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();