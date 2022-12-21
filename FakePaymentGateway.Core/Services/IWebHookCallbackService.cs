namespace FakePaymentGateway.Services;

public interface IWebHookCallbackService
{
    void Trigger(string endpoint, object notification);
    void Trigger(Uri endpoint, object notification);
}