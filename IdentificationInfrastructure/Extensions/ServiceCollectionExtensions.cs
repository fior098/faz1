using IdentificationApplication.Services;
using IdentificationDomain.Ports;
using IdentificationInfrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace IdentificationInfrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentificationInfrastructure(
        this IServiceCollection services,
        string postgresConnectionString,
        string redisConnectionString,
        TimeSpan tokenTtl)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConnectionString));

        services.AddSingleton<ITokenRepository>(sp =>
            new RedisTokenRepository(
                sp.GetRequiredService<IConnectionMultiplexer>(),
                sp.GetRequiredService<ILogger<RedisTokenRepository>>()));

        services.AddSingleton<IUserRepository>(sp =>
            new PostgresUserRepository(
                postgresConnectionString,
                sp.GetRequiredService<ILogger<PostgresUserRepository>>()));

        services.AddSingleton<IAuthenticationService>(sp =>
            new AuthenticationService(
                sp.GetRequiredService<IUserRepository>(),
                sp.GetRequiredService<ITokenRepository>(),
                tokenTtl));

        return services;
    }
}