using Hangfire;
using Microsoft.Extensions.Options;
using CryptoCreditCardRewards.Models.Settings;
using CryptoCreditCardRewards.Utilities;
using CryptoCreditCardRewards.Utilities.Enums;

namespace CryptoCreditCardRewards.API.Services.Hosted
{
    /// <summary>
    /// Base hosted service
    /// </summary>
    /// <typeparam name="T1">Implemented bys class</typeparam>
    /// <typeparam name="T2">Service options</typeparam>
    public abstract class BaseHostedService<T1, T2> : BackgroundService
        where T2 : BaseHostedServiceSettings
    {
        /// <summary>
        /// The class options
        /// </summary>
        protected readonly T2 _options;

        /// <summary>
        /// The class logger
        /// </summary>
        protected readonly ILogger<T1> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="options"></param>
        public BaseHostedService(ILogger<T1> logger, IOptions<T2> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        /// <summary>
        /// This is called on kick-off of applicaition
        /// </summary>
        /// <param name="action"></param>
        /// <param name="stoppingToken"></param>
        /// <returns>The task beimg executed</returns>
        protected async Task ExecuteSafelyAsync(Func<string> action, CancellationToken stoppingToken)
        {
            // Get the sleep time
            var sleepTime = _options.Interval.ParseFrequencyConfig(TimeUnit.MilliSeconds);

            // Calculate how long sleep for
            _logger.LogDebug($"{DateTime.UtcNow}|Sleep time for '{typeof(T1)}' is: {sleepTime} ms");

            // Check if the hosted service is used
            if (_options?.Enabled ?? false)
            {
                // Create a new thread to execute monitor
                await Task.Factory.StartNew(async () =>
                {
                    // Start the poll
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            // Monitor the bookings
                            action();
                        }
                        catch (Exception ex)
                        {
                            // Log to file - this will be a telegram error
                            _logger.LogCritical(ex.ToString());
                        }
                        finally
                        {
                            // Delay until next run
                            await Task.Delay((int)sleepTime);
                        }
                    }
                });
            }
        }
    }
}
