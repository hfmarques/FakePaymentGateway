using FakePaymentGateway.Entities;

namespace FakePaymentGateway.Repositories;

public interface IPersistenceRepository<T> where T : IPersistentEntity
{
    void Insert(T entity);

    void Update(T entity);

    long Delete(T entity);
}