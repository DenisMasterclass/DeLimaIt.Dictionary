
using Microsoft.Extensions.DependencyInjection;
using DeLimaIt.Dictionary.Application.Shared;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DeLimaIt.Dictionary.Application.Shared.Repository
{
    public static class SqlServerExtensions
    {
        public static IServiceCollection AddSqlServerDictionary(this IServiceCollection services, string connectionString, string sqlTimeoutKey)
        {
            services
                .AddScoped(_ => new DataContext(connectionString))
                .AddSqlHealthCheck(connectionString, sqlTimeoutKey);
            return services;
        }
        public static IServiceCollection AddSqlHealthCheck(this IServiceCollection services, string connectionString, string sqlTimeoutKey)
        {
            services.AddHealthChecks()
                .AddSqlServer(
                    connectionStringFactory: p => connectionString,
                    healthQuery: "SELECT 1;",
                    name: "Sql Server Dictionary Database",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "db", "sql", "sql server" },
                    timeout: TimeSpan.TryParse(sqlTimeoutKey, out var sqlServerTimeout) ? TimeSpan.FromSeconds(15) : sqlServerTimeout);
                );
            return services;
        }
    }
}
