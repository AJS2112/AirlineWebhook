using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AirlineSendAgent.Client;
using AirlineSendAgent.Data;
using AirlineSendAgent.Dtos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AirlineSendAgent.App
{
    internal class AppHost : IAppHost
    {
        private readonly SendAgentDbContext _context;
        private readonly IWebhookClient _webhookClient;
        public AppHost(SendAgentDbContext context, IWebhookClient webhookClient)
        {
            _context = context;
            _webhookClient = webhookClient;
        }

        public void Run()
        {
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName, exchange: "trigger", routingKey: "");

                var consumer = new EventingBasicConsumer(channel);
                Console.WriteLine("Listening on the message bus...");

                consumer.Received += async (ModuleHandle, ea) =>
                {
                    Console.WriteLine("Event is triggered!");

                    var body = ea.Body;
                    var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
                    var message = JsonSerializer.Deserialize<NotificationMessageDto>(notificationMessage);

                    var webhookToSend = new FlightDetailChangePayloadDto
                    {
                        WebhookType = message.WebhookType,
                        WebhookURI = String.Empty,
                        Secret = String.Empty,
                        Publisher = String.Empty,
                        OldPrice = message.OldPrice,
                        NewPrice = message.NewPrice,
                        FlightCode = message.FlightCode
                    };

                    foreach (var item in _context.WebhookSubscriptions.Where(s => s.WebhookType.Equals(message.WebhookType)))
                    {
                        webhookToSend.WebhookURI = item.WebhookURI;
                        webhookToSend.Secret = item.Secret;
                        webhookToSend.Publisher = item.WebhookPublisher;

                        await _webhookClient.SendWebNotificationAsync(webhookToSend);
                    }

                };

                channel.BasicConsume(queue: queueName, 
                            autoAck: true, 
                            consumer: consumer);

                Console.ReadLine();
            }
        }
    }
}
