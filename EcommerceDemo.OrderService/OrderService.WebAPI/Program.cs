using Ecommerce.SharedLibrary.DependencyInjection;
using OrderService.Application;
using OrderService.GrpcClients;
using OrderService.GrpcClients.Configuration;
using OrderService.GrpcClients.Interfaces;
using OrderService.GrpcClients.Services;
using OrderService.Infrastructure;
using OrderService.WebAPI.GrpcClients.Interfaces;
using OrderService.WebAPI.GrpcClients.Services;
using PromotionService.WebAPI.Protos;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

SharedServicesContainer.AddSharedServices(builder.Services, builder.Configuration, builder.Configuration["MySerilog:Filename"]!);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("UserOnly", policy =>
        policy.RequireAuthenticatedUser().RequireRole("Admin", "User"));

// Add services to the container.
builder.Services.AddGrpc();

// Configure gRPC client options
var grpcOptions = builder.Configuration.GetSection("GrpcClients").Get<GrpcClientOptions>() ?? new GrpcClientOptions();
builder.Services.Configure<GrpcClientOptions>(builder.Configuration.GetSection("GrpcClients"));

// Register Promotion Service gRPC client
builder.Services.AddGrpcClient<CouponService.CouponServiceClient>(options =>
{
    options.Address = new Uri(grpcOptions.PromotionGrpcServiceUrl ?? "http://localhost:5006");
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
    options.Address = new Uri(grpcOptions.ProductGrpcServiceUrl ?? "http://localhost:5005");
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
builder.Services.AddScoped<IProductGrpcClient, ProductGrpcClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}

app.UseRouting();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
