using AuthService.Application;
using AuthService.Infrastructure;
using Ecommerce.SharedLibrary.DependencyInjection;
using Ecommerce.SharedLibrary.Middlewares;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSharedServices(builder.Configuration, builder.Configuration["MySerilog:Filename"]!);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI();
}

app.UseMiddleware<ListenToOnlyApiGateway>();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
