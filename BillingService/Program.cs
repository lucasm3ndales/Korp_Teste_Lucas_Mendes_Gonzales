using Mapster;
using Microsoft.EntityFrameworkCore;
using Serilog;
using BillingService.Application.Common.Middlewares;
using BillingService.Config;
using BillingService.Infra.Data;
using BillingService.Presentation;
using System.Reflection;
using System.Text.Json;
using BillingService.Application.Common.Repositories;
using BillingService.Infra.Repositories;

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
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Billing Service API",
        Version = "v1",
        Description = "Documentação da API de Gerenciamento de Fatura."
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddScoped<IInvoiceNoteRepository, InvoiceNoteRepository>();

builder.Services.AddDbContext<BillingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

var stockServiceUrl = builder.Configuration["GrpcServices:StockService"];

builder.Services.AddGrpcClient<StockManager.Grpc.StockManager.StockManagerClient>(options =>
{
    options.Address = new Uri(stockServiceUrl);
});

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
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Billing Service API v1");
    });
}

app.UseSerilogRequestLogging();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.MapV1InvoiceNoteRoutes();

app.Run();

public partial class Program { }