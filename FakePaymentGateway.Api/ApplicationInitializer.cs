using FakePaymentGateway.Entities;
using MongoDB.Driver;
using RabbitMQ.Client;

namespace FakePaymentGateway;

public class ApplicationInitializer
{
    private readonly IMongoCollection<Account> collection;
    private readonly IModel rabbitMQModel;

    public ApplicationInitializer(IMongoCollection<Account> collection, IModel rabbitMQModel)
    {
        this.collection = collection;
        this.rabbitMQModel = rabbitMQModel;
    }

    public void Initialize()
    {
        InitializeDatabase();
        InitializeRabbitMQ();
    }

    private void InitializeDatabase()
    {
        collection.Database.DropCollection("Account");
        collection.Database.DropCollection("PaymentOrder");

        collection.InsertOne(new Account
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Active = true,
            Name = "Account 1",
            Balance = 0
        });
        collection.InsertOne(new Account
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
            Active = true,
            Name = "Account 2",
            Balance = 0
        });
        collection.InsertOne(new Account
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
            Active = true,
            Name = "Account 3",
            Balance = 0
        });
        collection.InsertOne(new Account
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
            Active = true,
            Name = "Account 4",
            Balance = 0
        });
    }

    private void InitializeRabbitMQ()
    {
        rabbitMQModel.QueueDeclare("payments", true, false, false);
        rabbitMQModel.QueueDeclare("payments_history", true, false, false);
        rabbitMQModel.QueueDeclare("notification", true, false, false);

        rabbitMQModel.ExchangeDeclare("payment_request", "topic", true, false);

        rabbitMQModel.QueueBind("payments", "payment_request", "");
        rabbitMQModel.QueueBind("payments_history", "payment_request", "");


    }

}