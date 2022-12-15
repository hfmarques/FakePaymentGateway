using FakePaymentGateway.Entities;
using MongoDB.Driver;

namespace FakePaymentGateway.Repositories;

public class MongoDbRepository<T> where T : IPersistentEntity
{
    protected IMongoCollection<T> Collection { get; }

    public MongoDbRepository(IMongoCollection<T> collection)
    {
        Collection = collection;
    }
}