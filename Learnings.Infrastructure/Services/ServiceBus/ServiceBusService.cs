using Azure.Messaging.ServiceBus;
using Learnings.Application.Services.ServiceBus;
using System.Text.Json;

namespace Learnings.Infrastructure.Services.ServiceBus
{
    public class ServiceBusService : IServiceBusService, IAsyncDisposable
    {
        private readonly ServiceBusClient _client;

        public ServiceBusService(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be empty", nameof(connectionString));

            _client = new ServiceBusClient(connectionString);
        }

        public async Task SendMessageAsync<T>(string queueName, T message)
        {
            ServiceBusSender sender = null;
            try
            {
                sender = _client.CreateSender(queueName);
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var jsonMessage = JsonSerializer.Serialize(message, options);

                Console.WriteLine($"[ServiceBus] Sending to queue: {queueName}");
                Console.WriteLine($"[ServiceBus] JSON: {jsonMessage}");

                var sbMessage = new ServiceBusMessage(jsonMessage)
                {
                    ContentType = "application/json",
                    CorrelationId = Guid.NewGuid().ToString()
                };

                await sender.SendMessageAsync(sbMessage);
                Console.WriteLine("[ServiceBus] ✓ Message sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ServiceBus] ✗ ERROR: {ex.GetType().Name} - {ex.Message}");
                throw;
            }
            finally
            {
                if (sender != null)
                    await sender.DisposeAsync();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_client != null)
            {
                await _client.DisposeAsync();
            }
        }
    }
}