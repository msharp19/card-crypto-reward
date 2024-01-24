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
    /// To be thrown for Bad Requests - 400
    /// </summary>
    public class BadRequestException : HttpActionValidationException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">A message for the exception</param>
        public BadRequestException(FailedReason failedReason, Property? property = null) : base(HttpStatusCode.Forbidden, failedReason, property)
        { }
    }
}
