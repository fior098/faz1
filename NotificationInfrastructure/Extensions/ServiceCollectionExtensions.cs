using NotificationApplication.Services;
using NotificationDomain.Ports;
using NotificationInfrastructure.Adapters;
using NotificationInfrastructure.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationInfrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));

        services.AddSingleton<INotificationSender, ConsoleNotificationSender>();
        services.AddSingleton<INotificationHandler, NotificationHandler>();
        services.AddHostedService<KafkaConsumerService>();

        return services;
    }
}