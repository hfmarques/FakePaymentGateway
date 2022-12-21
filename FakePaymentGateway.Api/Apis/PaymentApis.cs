using FakePaymentGateway.Entities;
using FakePaymentGateway.Repositories;
using FakePaymentGateway.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FakePaymentGateway.Apis;

public static class PaymentApis
{
    public static RouteGroupBuilder MapPayments(this IEndpointRouteBuilder routes)
    {
        var groupBuilder = routes.MapGroup("/payments");

        groupBuilder.WithTags("Payments");

        groupBuilder.MapGet("/info",
            () => $"{DateTime.Now.ToString()} - {Environment.OSVersion} - {Environment.MachineName}");
        
        groupBuilder.MapGet(
            "/{id:guid}", 
            Results<Ok<PaymentOrder>, NotFound>(
                [FromServices] IQueryRepository<PaymentOrder> paymentQuery, 
                Guid id) =>
            {
                var payment = paymentQuery.SingleOrDefault(x => x.Id == id);
                return payment is not null ? TypedResults.Ok(payment) : TypedResults.NotFound();
            });

        groupBuilder.MapPost("/", 
            Created<PaymentOrder> (
                [FromBody]IPaymentService paymentService, 
                [FromBody] PaymentOrder payment) =>
        {
            paymentService.ProcessPayment(payment);

            return TypedResults.Created($"/payments/{payment.Id}", payment);
        });
        
        return groupBuilder;
    }
}