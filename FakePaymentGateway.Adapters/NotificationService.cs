using System.Text;
using System.Text.Json;
using FakePaymentGateway.Services;
using RabbitMQ.Client;

namespace FakePaymentGateway;

public class NotificationService : INotificationService
{
    private readonly IModel _rabbitMqModel;

    public NotificationService(IModel rabbitMqModel)
    {
        _rabbitMqModel = rabbitMqModel;
    }

    public void Notify(string exchange, string routingKey, object notification)
    {
        var serializedContent =
            JsonSerializer.Serialize(notification, new JsonSerializerOptions {WriteIndented = true});

        var messageBodyBytes = Encoding.UTF8.GetBytes(serializedContent);
        
        var prop = _rabbitMqModel.CreateBasicProperties();

        _rabbitMqModel.BasicPublish(exchange, routingKey, prop, messageBodyBytes);
    }
}