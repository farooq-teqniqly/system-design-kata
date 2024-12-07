using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MessageProcessor;

/// <summary>
/// The main entry point for the MessageProcessor application.
/// </summary>
/// <param name="args">The command-line arguments.</param>
/// <returns>A task that represents the asynchronous operation.</returns>
public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddUserSecrets<Program>()
            .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            Log.Information("setting up the message processor...");

            var serviceBusConnectionString = configuration["ServiceBus:ConnectionString"];
            var topicName = configuration["ServiceBus:TopicName"];
            var subscriptionName = configuration["ServiceBus:SubscriptionName"];

            using (var loggerFactory = LoggerFactory.Create(builder => { builder.AddSerilog(); }))
            {
                var sbClient = new ServiceBusClient(serviceBusConnectionString);
                var sbProcessor = sbClient.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions());

                var messageProcessor = new MessageProcessor(loggerFactory.CreateLogger<MessageProcessor>());
                var handler = new ServiceBusMessageHandler(messageProcessor, loggerFactory.CreateLogger<ServiceBusMessageHandler>());

                sbProcessor.ProcessMessageAsync += handler.MessageHandler;
                sbProcessor.ProcessErrorAsync += handler.ErrorHandler;

                var cts = new CancellationTokenSource();

                AppDomain.CurrentDomain.ProcessExit += async (_, __) =>
                {
                    await cts.CancelAsync();
                    Log.Information("processor received SIGTERM - shutting down...");
                    await sbProcessor.StopProcessingAsync(cts.Token);
                    await sbClient.DisposeAsync();
                };

                Console.CancelKeyPress += async (_, e) =>
                {
                    e.Cancel = true;
                    await cts.CancelAsync();
                    await sbProcessor.StopProcessingAsync(cts.Token);
                    await sbClient.DisposeAsync();
                };

                await sbProcessor.StartProcessingAsync(cts.Token);

                Log.Information("processor is listening for messages...");

                while (true)
                {
                    await Task.Delay(1000, cts.Token);
                }
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, "an error occurred");
        }

    }
}
