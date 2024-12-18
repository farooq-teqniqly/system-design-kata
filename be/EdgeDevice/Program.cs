using Azure.Messaging.ServiceBus;
using Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace EdgeDevice;

/// <summary>
/// Represents the entry point of the application.
/// </summary>
public class Program
{
    private static readonly char[] _temperatureUnits = ['f', 'c'];
    /// <summary>
    /// The main method of the application.
    /// </summary>
    public static async Task Main()
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

            Log.Information("producer starting...");

            var serviceBusConnectionString = configuration["ServiceBus:ConnectionString"];
            var topicName = configuration["ServiceBus:TopicName"];
            var numberOfSensors = int.Parse(configuration["Producer:NumberOfSensors"]!);
            var sendInterval = int.Parse(configuration["Producer:SendIntervalMilliseconds"]!);

            using (var loggerFactory = LoggerFactory.Create(builder => { builder.AddSerilog(); }))
            {
                var logger = loggerFactory.CreateLogger<MessageProducer<TemperatureMessage>>();
                var sbClient = new ServiceBusClient(serviceBusConnectionString);
                var sender = sbClient.CreateSender(topicName);
                var producer = new MessageProducer<TemperatureMessage>(sender, logger);
                var random = new Random();

                var cts = new CancellationTokenSource();

                AppDomain.CurrentDomain.ProcessExit += (_, __) =>
                {
                    cts.Cancel();
                    Log.Information("producer received SIGTERM - shutting down...");
                };

                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                };


                while (true)
                {
                    for (var i = 0; i < numberOfSensors; i++)
                    {
                        var deviceId = $"sensor-temp-{i + 1}";
                        var unit = _temperatureUnits[random.Next(_temperatureUnits.Length)];

                        var message = new TemperatureMessage
                        {
                            DeviceId = deviceId,
                            Value = GenerateRandomTemperature(unit, random),
                            Unit = unit
                        };

                        await producer.SendMessageAsync(message, cts.Token);
                        Log.Debug($"waiting for {sendInterval}ms...");
                        await Task.Delay(sendInterval, cts.Token);
                    }
                }
            }

        }
        catch (Exception exception)
        {
            Log.Error(exception, "an error occurred");
        }

    }

    private static double GenerateRandomTemperature(char unit, Random random)
    {
        int min;
        int max;

        switch (unit)
        {
            case 'c':
                min = 15;
                max = 27;
                break;
            case 'f':
                min = 60;
                max = 80;
                break;
            default:
                throw new ArgumentException("invalid unit", nameof(unit));
        }

        return Math.Round(random.Next(min, max) + random.NextDouble(), 1);
    }
}
