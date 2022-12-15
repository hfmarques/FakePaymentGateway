using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace FakePaymentGateway.Entities;

public class PaymentOrder
{
    [BsonId(IdGenerator = typeof(CombGuidGenerator))]
    public required Guid Id { get; set; }
    public required string CorrelationId { get; set; }
    public required Guid AccountId { get; set; }
    public required decimal Amount { get; set; }
    public required string CardNumber { get; set; }
    public required int Cvc { get; set; }
    public required DateTime Validate { get; set; }
    public required string Name { get; set; }
    public string? WebHookEndpoint { get; set; }
    public required bool Processed { get; set; }
}