using Mapster;
using Microsoft.EntityFrameworkCore;
using Serilog;
using BillingService.Application.Common.Middlewares;
using BillingService.Config;
using BillingService.Infra.Data;
using BillingService.Presentation;
using System.Reflection;
using BillingService.Domain.ValueObjects;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddDbContext<BillingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonContext.Default);
});

TypeAdapterConfig<InvoiceNoteId, Guid>.NewConfig()
    .MapWith(id => id.Value);

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.MapBillingRoutes();

app.Run();

public partial class Program { }