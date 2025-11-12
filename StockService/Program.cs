using System.Reflection;
using System.Text.Json;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using StockService.Application.Common.Interceptors;
using StockService.Application.Common.Middlewares;
using StockService.Application.Common.Repositories;
using StockService.Application.Common.Services.TransactionManager;
using StockService.Infra.Data;
using StockService.Infra.Repositories;
using StockService.Presentation.Grpc;
using StockService.Presentation.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Stock Service API",
        Version = "v1",
        Description = "Documentação da API de Gerenciamento de Estoque."
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddScoped<GrpcGlobalExceptionInterceptor>();
builder.Services.AddGrpc(options => { options.Interceptors.Add<GrpcGlobalExceptionInterceptor>(); });

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

builder.Services.AddDbContext<StockDbContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
);

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IIdempotencyKeyRepository, IdempotencyKeyRepository>();
builder.Services.AddScoped<ITransactionManagerService, TransacationManagerService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "Stock Service API v1"); });
}

app.UseSerilogRequestLogging();
app.UseMiddleware<HttpGlobalExceptionMiddleware>();
app.UseCors("AllowAll");
app.MapGrpcService<StockManagerService>();
app.MapV1ProductRoutes();

app.Run();

public partial class Program
{
}