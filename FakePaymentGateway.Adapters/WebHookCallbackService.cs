using FakePaymentGateway.Services;

namespace FakePaymentGateway;

public class WebHookCallbackService : IWebHookCallbackService
{
    public void Trigger(Uri endpoint, object notification) => Trigger(endpoint, notification);

    public void Trigger(string endpoint, object notification)
    {
        var client = new HttpClient();

        var serializedContent = System.Text.Json.JsonSerializer.Serialize(notification, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });

        var stringContent = new StringContent(serializedContent);

        client.PostAsync(endpoint, stringContent).GetAwaiter().GetResult();
    }
}