using System.Linq.Expressions;
using FakePaymentGateway.Entities;

namespace FakePaymentGateway.Repositories;

public interface IQueryRepository<T> where T : IPersistentEntity
{
    T SingleOrDefault(Expression<Func<T, bool>> predicate);
}