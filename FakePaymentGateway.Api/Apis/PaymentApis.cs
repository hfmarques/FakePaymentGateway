namespace FakePaymentGateway.Apis;

public static class PaymentApis
{
    public static RouteGroupBuilder MapPayments(this IEndpointRouteBuilder routes)
    {
        var groupBuilder = routes.MapGroup("/payments");

        groupBuilder.WithTags("Payments");

        groupBuilder.MapGet("/info",
            () => $"{DateTime.Now.ToString()} - {Environment.OSVersion} - {Environment.MachineName}");

        return groupBuilder;
    }
}