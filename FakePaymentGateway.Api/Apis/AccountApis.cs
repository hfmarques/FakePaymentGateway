using FakePaymentGateway.Entities;
using FakePaymentGateway.Repositories;
using FakePaymentGateway.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FakePaymentGateway.Apis;

public static class AccountApis
{
    public static RouteGroupBuilder MapAccounts(this IEndpointRouteBuilder routes)
    {
        var groupBuilder = routes.MapGroup("/accounts");

        groupBuilder.WithTags("Accounts");
        
        groupBuilder.MapGet(
            "/{id:guid}", 
            Results<Ok<Account>, NotFound>(
                [FromServices] IQueryRepository<Account> accountQuery, 
                Guid id) =>
            {
                var account = accountQuery.SingleOrDefault(x => x.Id == id);
                return account is not null ? TypedResults.Ok(account) : TypedResults.NotFound();
            });
        
        groupBuilder.MapPost("/", 
            Created<Account> (
                [FromServices] IPersistenceRepository<Account> accountPersistence, 
                [FromBody] Account account) =>
            {
                account.Id = Guid.NewGuid();
                
                accountPersistence.Insert(account);
        
                return TypedResults.Created($"/payments/{account.Id}", account);
            });
        
        return groupBuilder;
    }
}