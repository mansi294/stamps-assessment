using TakeHome.Api.Repositories;
using TakeHome.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IRateCalculator, RateCalculator>();
builder.Services.AddSingleton<IShipmentRepository, InMemoryShipmentRepository>();

const string ClientDevCorsPolicy = "ClientDev";
builder.Services.AddCors(options =>
{
    options.AddPolicy(ClientDevCorsPolicy, policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(ClientDevCorsPolicy);
app.UseAuthorization();

app.MapControllers();

app.Run();
