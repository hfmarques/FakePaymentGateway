using System.Linq.Expressions;
using FakePaymentGateway.Entities;
using MongoDB.Driver;

namespace FakePaymentGateway.Repositories;

public class MongoDbQueryRepository<T> : MongoDbRepository<T>, IQueryRepository<T> where T : IPersistentEntity
{
    public MongoDbQueryRepository(IMongoCollection<T> collection) : base(collection)
    {
    }

    public virtual T SingleOrDefault(Expression<Func<T, bool>> predicate) => Collection.Find(predicate).SingleOrDefault();
}