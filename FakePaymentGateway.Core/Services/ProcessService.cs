using FakePaymentGateway.Entities;
using FakePaymentGateway.Repositories;

namespace FakePaymentGateway.Services;

public class ProcessService : IProcessService
{
    private readonly IPersistenceRepository<PaymentOrder> _paymentPersistence;
    private readonly IPersistenceRepository<Account> _accountPersistence;
    private readonly IWebHookCallbackService _webHookCallbackService;
    private readonly IConsumer<PaymentOrder> _consumer;
    private readonly IQueryRepository<Account> _accountQueryRepository;

    public ProcessService(
        IPersistenceRepository<PaymentOrder> paymentPersistence, 
        IPersistenceRepository<Account> accountPersistence, 
        IWebHookCallbackService webHookCallbackService, 
        IConsumer<PaymentOrder> consumer, IQueryRepository<Account> accountQueryRepository)
    {
        _paymentPersistence = paymentPersistence;
        _accountPersistence = accountPersistence;
        _webHookCallbackService = webHookCallbackService;
        _consumer = consumer;
        _accountQueryRepository = accountQueryRepository;
    }

    public void Consume()
    {
        _consumer.Start("payments", ProcessPayment);
    }

    private void ProcessPayment(PaymentOrder payment)
    {
        var account = _accountQueryRepository.SingleOrDefault(it => it.Id == payment.AccountId);

        if (account == null)
        {
            throw new InvalidOperationException("Account not found");
        }

        var executed = false;
        var message = "";
        
        //10 is denied to exemplify a unauthorized 
        if (payment.Amount != 10)
        {
            account.Balance += payment.Amount;
            payment.Processed = true;

            _paymentPersistence.Update(payment);
            _accountPersistence.Update(account);

            executed = true;
            message = "Processed";
        }
        else
        {
            executed = false;
            message = "Amount = 10 (Unauthorized)";
        }

        if (!string.IsNullOrWhiteSpace(payment.WebHookEndpoint))
        {
            _webHookCallbackService.Trigger(payment.WebHookEndpoint, new {
                payment.CorrelationId,
                Executed = executed,
                Message = message
            });
        }
    }
}