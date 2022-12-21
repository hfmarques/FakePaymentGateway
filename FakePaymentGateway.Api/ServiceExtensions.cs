using FakePaymentGateway.Entities;
using FakePaymentGateway.Repositories;
using FakePaymentGateway.Services;
using MongoDB.Driver;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace FakePaymentGateway;

public static class ServiceExtensions
{
    private static void AddTransientWithRetry<TService, TException>(this IServiceCollection services,
        Func<IServiceProvider, TService> implementationFactory,
        int retryCount = 3)
        where TService : class
        where TException: Exception
    {
        services.AddTransient<TService>(sp => {

            var service = default(TService);
            var policy = Policy
                .Handle<TException>()
                .WaitAndRetry(retryCount, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                );

            policy.Execute(() =>
            {
                service = implementationFactory(sp);
            });

            return service;
        });
    }

    public static void InitRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient(_ => new ConnectionFactory
        {
            HostName = configuration.GetSection("Connections:RabbitMQ:Server").Value,
            Port = configuration.GetSection("Connections:RabbitMQ:Port").Get<int>(),
            UserName = configuration.GetSection("Connections:RabbitMQ:UserName").Value,
            Password = configuration.GetSection("Connections:RabbitMQ:Password").Value,
        });

        services.AddTransientWithRetry<IConnection, BrokerUnreachableException>(
            sp => sp.GetRequiredService<ConnectionFactory>().CreateConnection(), 3);

        services.AddTransient(sp => sp.GetRequiredService<IConnection>().CreateModel());

        services.AddTransient<IConsumer<PaymentOrder>>(sp=> 
            new ReliableQueueConsumer<PaymentOrder>(sp.GetRequiredService<IModel>(), 3)
        );
    }

    public static void InitMongoDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<MongoClient>(sp =>
        {
            var serverName = configuration.GetSection("Connections:Mongo:Server").Value;
            var port = configuration.GetSection("Connections:Mongo:Port").Value;
            var username = configuration.GetSection("Connections:Mongo:Username").Value;
            var password = configuration.GetSection("Connections:Mongo:Password").Value;

            return new MongoClient($"mongodb://{username}:{password}@{serverName}:{port}");
        });

        services.AddTransient(sp =>
        {
            var databaseName = configuration.GetSection("Connections:Mongo:DB").Value;
            return sp.GetRequiredService<MongoClient>().GetDatabase(databaseName);
        });

        services.AddTransient(sp => sp.GetRequiredService<IMongoDatabase>().GetCollection<PaymentOrder>("PaymentOrder"));
        services.AddTransient(sp => sp.GetRequiredService<IMongoDatabase>().GetCollection<Account>("Account"));

        services.AddTransient<IQueryRepository<PaymentOrder>, MongoDbQueryRepository<PaymentOrder>>();
        services.AddTransient<IQueryRepository<Account>, MongoDbQueryRepository<Account>>();

        services.AddTransient<IPersistenceRepository<PaymentOrder>, MongoDbPersistenceRepository<PaymentOrder>>();
        services.AddTransient<IPersistenceRepository<Account>, MongoDbPersistenceRepository<Account>>();

        services.AddTransient<IPaymentService, PaymentService>();
        services.AddTransient<INotificationService, NotificationService>();
    }
}