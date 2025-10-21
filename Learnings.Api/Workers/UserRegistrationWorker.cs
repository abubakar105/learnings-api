using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace Learnings.Api.Workers;

public class UserRegistrationWorker : BackgroundService
{
    private readonly ServiceBusProcessor _processor;
    private readonly ILogger<UserRegistrationWorker> _logger;

    public UserRegistrationWorker(
        IConfiguration configuration,
        ILogger<UserRegistrationWorker> logger)
    {
        var connectionString = configuration["ServiceBus:ConnectionString"];
        var client = new ServiceBusClient(connectionString);

        _processor = client.CreateProcessor("user-registration-queue", new ServiceBusProcessorOptions
        {
            MaxConcurrentCalls = 1,
            AutoCompleteMessages = false,
            MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(5)
        });

        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting UserRegistrationWorker...");
        await _processor.StartProcessingAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        await _processor.StopProcessingAsync();
        _logger.LogInformation("UserRegistrationWorker stopped.");
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var body = args.Message.Body.ToString();
            _logger.LogInformation($"Received message: {body}");

            var data = JsonSerializer.Deserialize<UserRegistrationMessage>(body);
            _logger.LogInformation($"Message processed for user: {data.Email}");

            // Logic App will handle the actual email sending
            // Just complete the message here
            await args.CompleteMessageAsync(args.Message);

            _logger.LogInformation($"Message completed for: {data.Email}");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize message. Moving to dead-letter queue.");
            await args.DeadLetterMessageAsync(args.Message, "DeserializationError", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message. Will retry.");
            await args.AbandonMessageAsync(args.Message);
        }
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception,
            $"Service Bus error - Source: {args.ErrorSource}, Entity: {args.EntityPath}");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.CloseAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}

public record UserRegistrationMessage(
    string UserId,
    string Email,
    string Name,
    DateTime RegisteredAt
);