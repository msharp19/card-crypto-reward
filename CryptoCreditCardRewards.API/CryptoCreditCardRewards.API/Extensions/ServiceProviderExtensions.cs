using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using CryptoCreditCardRewards.API.Services.Errors;
using CryptoCreditCardRewards.API.Services.Errors.Interfaces;
using CryptoCreditCardRewards.Models.Errors;

namespace CryptoCreditCardRewards.API.Extensions
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
            return services.AddDbContext<T>((serviceProvider, optionsBuilder) =>
            {
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.UseSqlServer(connectionString, options =>
                {
                    // Enable retry with max count of 10
                    options.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null
                    // Command timeout of 60s 
                    ).CommandTimeout(60);
                }).UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
            }, ServiceLifetime.Transient);
        }

        /// <summary>
        /// Sets NeoOptimas authorization policies (roles)
        /// </summary>
        /// <param name="services">The service provider</param>
        /// <param name="config">Platform configuration</param>
        /// <returns>The service collection with set policies</returns>
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services, IConfiguration config)
        {
            // Set up the policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("CryptoCreditCardRewards-access",
                        authBuilder =>
                        {
                            // Ensure user is authenticated
                            authBuilder.RequireAuthenticatedUser();
                            // Use JWT Bearer scheme
                            authBuilder.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                        }
                    );
            });

            return services;
        }

        /// <summary>
        /// Sets up the error mapping service
        /// </summary>
        /// <param name="services">The service provider</param>
        public static IServiceCollection LoadErrorMappings(this IServiceCollection services)
        {
            // Get the mappings
            var mappingJson = File.ReadAllText("failedresponsemappings.json");

            // Convert to obj
            var mappings = JsonConvert.DeserializeObject<List<ErrorMap>>(mappingJson);

            services.AddTransient<IFailedResponseMappingService, FailedResponseMappingService>(s =>
                new FailedResponseMappingService(s.GetService<IWebHostEnvironment>(), s.GetService<ILogger<FailedResponseMappingService>>(), Options.Create(mappings), "Development")
            );

            return services;
        }
    }
}
