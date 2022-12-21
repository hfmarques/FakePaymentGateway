using Polly;
using RabbitMQ.Client;

namespace FakePaymentGateway;

public class ReliableQueueConsumer<T> : QueueConsumer<T>, IConsumer<T>
{
    private readonly int _retryCount;
    public ReliableQueueConsumer(IModel rabbitMqModel, int retryCount) : base(rabbitMqModel)
    {
        this._retryCount = retryCount;
    }

    protected override void Dispatch(T? treatedObject, Action<T> action)
    {
        var policy = Policy
            .Handle<Exception>()
            .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        policy.Execute(() =>
        {
            action(treatedObject);
        });
    }
}