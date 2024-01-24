using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using CryptoCreditCardRewards.API.Services.Errors.Interfaces;
using CryptoCreditCardRewards.Models.Enums;
using CryptoCreditCardRewards.Models.Errors;
using CryptoCreditCardRewards.Utilities;

namespace CryptoCreditCardRewards.API.Services.Errors
{
    /// <summary>
    /// Maps requests/operations using app config settings
    /// </summary>
    public class FailedResponseMappingService : IFailedResponseMappingService
    {
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Configuration for the failed response reasons
        /// </summary>
        private Dictionary<FailedReason, string>? _failedResponseMappings;

        /// <summary>
        /// Class logger
        /// </summary>
        private readonly ILogger<FailedResponseMappingService> _loggingService;

        /// <summary>
        /// Instance of API
        /// </summary>
        private readonly string _instance;

        /// <summary>
        /// Constructor
        /// </summary>
        public FailedResponseMappingService(IWebHostEnvironment environment, ILogger<FailedResponseMappingService> loggingService, IOptions<List<ErrorMap>>? failedResponseMappings, string instance)
        {
            _loggingService = loggingService;
            _environment = environment;
            _instance = instance;
            _failedResponseMappings = failedResponseMappings?.Value?.ToDictionary(x => ConvertToFailedReason(x.Key), y => y.Value);
        }

        /// <summary>
        /// Safely convert a string to a failed reason. If for any reason it cant be mapped, it is returned as none
        /// </summary>
        /// <param name="key">The value to parse and convert to enum</param>
        /// <returns>Mapped enum</returns>
        private FailedReason ConvertToFailedReason(string key)
        {
            return (FailedReason)Enum.Parse(typeof(FailedReason), key);
        }

        /// <summary>
        /// Maps failed responses from configuration (text string)
        /// </summary>
        /// <param name="reason">The generic reason to map</param>
        /// <param name="status">The failed response status</param>
        /// <param name="traceId">The trace code</param>
        /// <param name="relatedProperty">The related property</param>
        /// <param name="stackTrace">The error debug message (if running in dev)</param>
        /// <returns>A human readable response string for type of error (if exists in configuration)</returns>
        public CryptoCreditCardRewardsProblemDetails Map(FailedReason reason, HttpStatusCode status, string traceId, Property? relatedProperty, string stackTrace = null)
        {
            var problemDetails = new CryptoCreditCardRewardsProblemDetails();

            if (_failedResponseMappings.TryGetValue(reason, out var errorMessageMapping))
            {
                problemDetails.Title = reason.ToString();
                problemDetails.Stacktrace = stackTrace;
                problemDetails.Status = (int)status;
                problemDetails.TraceId = traceId;
                problemDetails.Type = status.ToString();
                problemDetails.Instance = _instance;

                problemDetails.Errors = ErrorUtility.BuildErrors((relatedProperty?.ToString()?.LowercaseFirstLetter() ?? reason.ToString(), errorMessageMapping));

                return problemDetails;
            }

            return null;
        }
    }
}
