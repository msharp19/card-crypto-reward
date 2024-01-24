using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Models.Exceptions
{
    /// <summary>
    /// To be thrown for Unprocessable Entity Requests - 422
    /// </summary>
    public class UnprocessableEntityException : HttpActionValidationException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">A message for the exception</param>
        public UnprocessableEntityException(FailedReason failedReason, Property? property = null) : base(HttpStatusCode.UnprocessableEntity, failedReason, property)
        { }
    }
}
