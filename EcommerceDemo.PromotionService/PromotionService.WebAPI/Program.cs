using Microsoft.AspNetCore.Server.Kestrel.Core;
using PromotionService.Application;
using PromotionService.Infrastructure;
using PromotionService.WebAPI.Endpoints;
using PromotionService.WebAPI.Grpc;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddOpenApi();
builder.Services.AddGrpc();

// Register Infrastructure
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5003, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });
    options.ListenAnyIP(5006, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();
app.UseAuthorization();

// Map endpoints
app.MapGrpcService<CouponGrpcService>();
app.MapControllers();
app.MapCouponEndpoints();

app.Run();
