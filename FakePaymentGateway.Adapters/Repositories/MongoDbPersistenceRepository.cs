using FakePaymentGateway.Entities;
using MongoDB.Driver;

namespace FakePaymentGateway.Repositories;

public class MongoDbPersistenceRepository<T> : MongoDbRepository<T>, IPersistenceRepository<T> where T : IPersistentEntity
{
    public MongoDbPersistenceRepository(IMongoCollection<T> collection) : base(collection)
    {
    }

    public virtual void Insert(T entity) => Collection.InsertOne(entity);
    
    public virtual void Update(T entity) => Collection.ReplaceOne(it => it.Id == entity.Id, entity);
    
    public virtual long Delete(T entity) => Collection.DeleteOne(it => it.Id == entity.Id).DeletedCount;
}