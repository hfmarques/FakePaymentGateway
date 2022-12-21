using FakePaymentGateway;
using FakePaymentGateway.Apis;
using FakePaymentGateway.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.InitRabbitMQ(builder.Configuration);
builder.Services.InitMongoDependencies(builder.Configuration);

builder.Services.AddSingleton<ApplicationInitializer>();
builder.Services.AddTransient<ProcessService>();
builder.Services.AddSingleton<IWebHookCallbackService, WebHookCallbackService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Map("/", () => Results.Redirect("/swagger"));

app.Services.GetRequiredService<ProcessService>().Consume();

app.MapPayments();

app.UseHttpsRedirection();

app.Run();