using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace FakePaymentGateway.Entities;

public class Account : IPersistentEntity
{
    [BsonId(IdGenerator = typeof(CombGuidGenerator))]
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required bool Active { get; set; }
    public required decimal Balance { get; set; }
}