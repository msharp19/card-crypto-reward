using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;
using CryptoCreditCardRewards.API.Services.Errors;
using CryptoCreditCardRewards.Utilities;


namespace CryptoCreditCardRewards.API.Helpers
{
    /// <summary>
    /// Model state validation helper
    /// </summary>
    public class ModelStateValidationErrorHelper
    {
        /// <summary>
        /// Builds a bad request from the model state
        /// </summary>
        /// <param name="modelStateDictionary">The model state validation results</param>
        /// <returns>A bad request</returns>
        public static BadRequestObjectResult BuildBadRequest(ModelStateDictionary modelStateDictionary)
        {
            // Get errors
            var errors = modelStateDictionary.Keys
                .SelectMany(key => modelStateDictionary[key].Errors.Select(x => (key.LowercaseFirstLetter(), x.ErrorMessage)))
                .ToList();

            // Transform them to our format
            var transformedErrors = ErrorUtility.BuildErrors(errors.ToArray());

            // Build the error response
            var problemDetails = new CryptoCreditCardRewardsProblemDetails()
            {
                Errors = transformedErrors,
                Stacktrace = null,
                Status = (int)HttpStatusCode.BadRequest,
                Title = "One or more validation errors occurred.",
                TraceId = Guid.NewGuid().ToString(),
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            };

            // Return the resultant object
            return new BadRequestObjectResult(problemDetails);
        }
    }
}
