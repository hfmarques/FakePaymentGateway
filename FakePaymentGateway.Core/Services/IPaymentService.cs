using FakePaymentGateway.Entities;

namespace FakePaymentGateway.Services;

public interface IPaymentService
{
    void ProcessPayment(PaymentOrder paymentOrder);
}