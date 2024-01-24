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
    /// To be thrown for Unauthorized Requests - 401
    /// </summary>
    public class UnauthorizedException : HttpActionValidationException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">A message for the exception</param>
        public UnauthorizedException(FailedReason failedReason, Property? property = null) : base(HttpStatusCode.Forbidden, failedReason, property)
        { }
    }
}
