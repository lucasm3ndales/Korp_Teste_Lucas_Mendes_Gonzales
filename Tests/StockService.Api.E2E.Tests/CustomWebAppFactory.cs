using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions; 
using StockService.Infra.Data;

namespace StockService.Api.E2E.Tests;

public class CustomWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSolutionRelativeContentRoot("StockService");

        builder.ConfigureServices(services =>
        {
            // --- INÍCIO DA CORREÇÃO DEFINITIVA ---
            
            // RemoveAll garante que CADA registro de serviço seja removido
            services.RemoveAll(typeof(DbContextOptions<StockDbContext>));
            services.RemoveAll(typeof(StockDbContext));
            services.RemoveAll(typeof(DbContextOptions)); // Remove o não-genérico

            // Remove qualquer DbConnection anterior (se houver)
            var dbConnectionDescriptor = services.SingleOrDefault(d => 
                d.ServiceType == typeof(DbConnection));
            if (dbConnectionDescriptor != null)
                services.Remove(dbConnectionDescriptor);

            // --- FIM DA CORREÇÃO ---
            
            // Agora, adiciona com segurança as novas implementações em memória
            services.AddSingleton<DbConnection>(container =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();
                return connection;
            });

            services.AddDbContext<StockDbContext>((container, options) =>
            {
                var connection = container.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });
        });
    }
}