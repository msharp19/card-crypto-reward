using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using CryptoCreditCardRewards.Models.Errors;

namespace CryptoCreditCardRewards.CreditCardRewardIssuerFunction
{
    /// <summary>
    /// Extension methods for IServiceProvider
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Sets a database context into the service collection
        /// </summary>
        /// <typeparam name="T">The type of context (T:DbContext)</typeparam>
        /// <param name="services">The service provider</param>
        /// <param name="connectionString">The connection string of the connection</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection SetContext<T>(this IServiceCollection services, string connectionString)
           where T : DbContext
        {
            // Add db context pool using SqlServer to the service collection
            return services.AddDbContextPool<T>(optionsBuilder =>
                           optionsBuilder.UseSqlServer(
                                  connectionString,
                                  options => options
                                  // Enable retry with max count of 10
                                     .EnableRetryOnFailure(
                                         maxRetryCount: 10,
                                         maxRetryDelay: TimeSpan.FromSeconds(5),
                                         errorNumbersToAdd: null)
                                     // Timeout occours after 60 seconds
                                     .CommandTimeout(60)).UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                           );
        }
    }
}
