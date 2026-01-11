using NotificationInfrastructure.Extensions;

IHost host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddJsonFile(string.Format("appsettings.{0}.json", context.HostingEnvironment.EnvironmentName), optional: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddNotificationInfrastructure(context.Configuration);
    })
    .Build();

await host.RunAsync();