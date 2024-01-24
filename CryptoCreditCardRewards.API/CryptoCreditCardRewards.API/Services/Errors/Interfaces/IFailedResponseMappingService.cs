using Microsoft.AspNetCore.Mvc;
using System.Net;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.API.Services.Errors.Interfaces
{
    /// <summary>
    /// Contract for a failed response mapping service
    /// </summary>
    public interface IFailedResponseMappingService
    {
        /// <summary>
        /// Maps failed responses from configuration (text string)
        /// </summary>
        /// <param name="reason">The generic reason to map</param>
        /// <param name="status">The failed response status</param>
        /// <param name="traceId">The trace code</param>
        /// <param name="relatedProperty">A related property to the error (if exists). If it doesn't exist then the message mapping code is provided</param>
        /// <param name="stackTrace">The error debug message (if running in dev)</param>
        /// <returns>A human readable response string for type of error (if exists in configuration)</returns>
        CryptoCreditCardRewardsProblemDetails Map(FailedReason reason, HttpStatusCode status, string traceId, Property? relatedProperty, string stackTrace = null);
    }
}
