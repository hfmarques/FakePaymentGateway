namespace FakePaymentGateway.Services;

public interface INotificationService
{
    void Notify(string exchange, string routingKey, object notification);
}