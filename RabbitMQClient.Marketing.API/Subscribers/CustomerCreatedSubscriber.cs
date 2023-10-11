using MessagingEvents.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace RabbitMQClient.Marketing.API.Subscribers
{
    public class CustomerCreatedSubscriber : IHostedService
    {
        private readonly IModel _channel;

        const string EXCHANGE = "rabbitmq";
        const string CUSTOMER_CREATED_QUEUE = "cursorabbitmq-client";

        public CustomerCreatedSubscriber()
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            var connection = connectionFactory.CreateConnection("curso-rabbitmq-client-consumer");

            _channel = connection.CreateModel();

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (sender, eventArgs) =>
            {
                var contentArray = eventArgs.Body.ToArray();
                var contentString = Encoding.UTF8.GetString(contentArray);

                var @event = JsonSerializer.Deserialize<CustomerCreated>(contentString);

                Console.WriteLine($"Messagem recebida: {contentString}");

                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            _channel.BasicConsume(CUSTOMER_CREATED_QUEUE, false, consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
           throw new NotImplementedException();
        }
    }
}

