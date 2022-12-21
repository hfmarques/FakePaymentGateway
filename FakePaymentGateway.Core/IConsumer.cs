namespace FakePaymentGateway;

public interface IConsumer<T>
{
    void Start(string queueName, Action<T> action);
}