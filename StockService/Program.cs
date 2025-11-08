using System.Reflection;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StockService.Application.Common.Interceptors;
using StockService.Application.Common.Middlewares;
using StockService.Application.Repositories;
using StockService.Config;
using StockService.Infra.Data;
using StockService.Infra.Repositories;
using StockService.Presentation;
using StockService.Presentation.Grpc;

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

TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonContext.Default);
});

builder.Services.AddDbContext<StockDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddScoped<GrpcGlobalExceptionInterceptor>();

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<GrpcGlobalExceptionInterceptor>();
});

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseMiddleware<HttpGlobalExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.MapGrpcService<StockManagerService>();
app.MapProductRoutes();

app.Run();

public partial class Program { }