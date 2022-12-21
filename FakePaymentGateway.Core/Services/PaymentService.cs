using FakePaymentGateway.Entities;
using FakePaymentGateway.Repositories;

namespace FakePaymentGateway.Services;

public class PaymentService : IPaymentService
{
    private readonly INotificationService _notificationService;
    private readonly IQueryRepository<Account> _accountQuery;
    private readonly IPersistenceRepository<PaymentOrder> _paymentPersistence;

    public PaymentService(INotificationService notificationService, IQueryRepository<Account> accountQuery, IPersistenceRepository<PaymentOrder> paymentPersistence)
    {
        _notificationService = notificationService;
        _accountQuery = accountQuery;
        _paymentPersistence = paymentPersistence;
    }


    public void ProcessPayment(PaymentOrder paymentOrder)
    {
        var account = _accountQuery.SingleOrDefault(it => it.Id == paymentOrder.AccountId);

        if (account is null)
            throw new InvalidOperationException();

        if (!string.IsNullOrWhiteSpace(paymentOrder.WebHookEndpoint) &&
            string.IsNullOrWhiteSpace(paymentOrder.CorrelationId))
            throw new InvalidOperationException();

        paymentOrder.Processed = false;
        
        _paymentPersistence.Insert(paymentOrder);
        
        _notificationService.Notify("payment_request", "", paymentOrder);
    }
}