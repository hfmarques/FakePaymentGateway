using System.Reflection;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FakePaymentGateway;

public class QueueConsumer<T> : IConsumer<T>
{
    private readonly IModel _rabbitMqModel;

    public QueueConsumer(IModel rabbitMqModel)
    {
        _rabbitMqModel = rabbitMqModel;
    }

    public void Start(string queueName, Action<T> action)
    {
        var consumer = new EventingBasicConsumer(_rabbitMqModel);

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();

            var message = Encoding.UTF8.GetString(body);

            var treatedObject = default(T);

            try
            {
                treatedObject = JsonSerializer.Deserialize<T>(message);
            }
            catch (Exception e)
            {
                _rabbitMqModel.BasicReject(ea.DeliveryTag, true);
                throw;
            }

            try
            {
                Dispatch(treatedObject, action);
                _rabbitMqModel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception e)
            {
                _rabbitMqModel.BasicNack(ea.DeliveryTag, false, true);
                throw;
            }
        };

        _rabbitMqModel.BasicConsume(queue: queueName, autoAck: false, consumer);
    }

    protected virtual void Dispatch(T? treatedObject, Action<T> action) => action(treatedObject);
}