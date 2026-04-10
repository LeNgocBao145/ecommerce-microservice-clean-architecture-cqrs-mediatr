using Ecommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.RateLimiting;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builderContext =>
     {
         // Lấy key từ config một lần duy nhất
         var internalKey = builder.Configuration["Authentication:Key"];

         builderContext.AddRequestTransform(async transformContext =>
         {
             // Thêm Header trực tiếp vào ProxyRequest (Request sẽ gửi đi)
             transformContext.ProxyRequest.Headers.Add("X-Internal-Gateway-Token", internalKey);
         });
     });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("authenticated", policy => policy.RequireAuthenticatedUser())
    .AddPolicy("AdminOnly", policy =>
        policy.RequireAuthenticatedUser().RequireRole("Admin"))
    .AddPolicy("UserOnly", policy =>
        policy.RequireAuthenticatedUser().RequireRole("User", "Admin"));

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.Window = TimeSpan.FromSeconds(10);
        options.PermitLimit = 5;
    });
});


SharedServicesContainer.AddSharedServices(builder.Services, builder.Configuration, builder.Configuration["MySerilog:ApiGateway"]!);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseRateLimiter();

app.MapReverseProxy();

app.Run();
