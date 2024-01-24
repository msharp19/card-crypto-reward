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
    /// To be thrown for Forbidden Requests - 403
    /// </summary>
    public class ForbidException : HttpActionValidationException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">A message for the exception</param>
        public ForbidException(FailedReason failedReason, Property? property = null) : base(HttpStatusCode.Forbidden, failedReason, property)
        { }
    }
}
