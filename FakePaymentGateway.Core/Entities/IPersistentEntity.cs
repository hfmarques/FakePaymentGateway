namespace FakePaymentGateway.Entities;

public interface IPersistentEntity
{
    public Guid Id { get; set; }
}