using OrderService.GrpcClients;
using OrderService.GrpcClients.Configuration;
using OrderService.WebAPI.GrpcClients.Interfaces;
using OrderService.WebAPI.GrpcClients.Services;
using PromotionService.WebAPI.Protos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

// Configure gRPC client options
var grpcOptions = builder.Configuration.GetSection("GrpcClients").Get<GrpcClientOptions>() ?? new GrpcClientOptions();
builder.Services.Configure<GrpcClientOptions>(builder.Configuration.GetSection("GrpcClients"));

// Register Promotion Service gRPC client
builder.Services.AddGrpcClient<CouponService.CouponServiceClient>(options =>
{
    options.Address = new Uri(grpcOptions.PromotionServiceUrl ?? "https://localhost:5003");
})
.ConfigureChannel(options =>
{
    options.HttpHandler = new SocketsHttpHandler
    {
        KeepAlivePingDelay = TimeSpan.FromSeconds(30),
        KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
        EnableMultipleHttp2Connections = true
    };
});

builder.Services.AddGrpcClient<Product.ProductClient>(options =>
{
    options.Address = new Uri(grpcOptions.ProductServiceUrl ?? "https://localhost:5001");
})
.ConfigureChannel(options =>
{
    options.HttpHandler = new SocketsHttpHandler
    {
        KeepAlivePingDelay = TimeSpan.FromSeconds(30),
        KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
        EnableMultipleHttp2Connections = true
    };
});

// Register custom gRPC client wrapper
builder.Services.AddScoped<IPromotionGrpcClient, PromotionGrpcClient>();

builder.Services.AddGrpc();

var app = builder.Build();

app.Run();
