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
    /// To be thrown for Not Found Requests - 404
    /// </summary>
    public class NotFoundException : HttpActionValidationException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">A message for the exception</param>
        public NotFoundException(FailedReason failedReason, Property? property = null) : base(HttpStatusCode.NotFound, failedReason, property)
        { }
    }
}
