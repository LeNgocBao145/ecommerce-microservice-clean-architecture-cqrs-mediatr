using Ecommerce.SharedLibrary.DependencyInjection;
using Ecommerce.SharedLibrary.Middlewares;
using ProductService.Application;
using ProductService.Domain.Exceptions;
using ProductService.Infrastructure;
using ProductService.WebAPI.Grpc;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSharedServices(builder.Configuration, builder.Configuration["MySerilog:Filename"]!);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy =>
        policy.RequireAuthenticatedUser().RequireRole("Admin"));

builder.Services.AddGrpc();

var app = builder.Build();

app.UseMiddleware<ListenToOnlyApiGateway>();
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapGrpcService<ProductGrpcService>();

app.MapControllers();

app.Run();
