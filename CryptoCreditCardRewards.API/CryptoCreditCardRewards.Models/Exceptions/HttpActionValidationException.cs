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
    /// Base Http validation exception
    /// </summary>
    public abstract class HttpActionValidationException : Exception
    {
        /// <summary>
        /// The status code of the action
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// The validation error
        /// </summary>
        public FailedReason FailedReason { get; set; }

        /// <summary>
        /// A model property tied to the error (nullable since not every execption can be tied to an input property)
        /// </summary>
        public Property? Property { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="statusCode">The http status code to associate with error</param>
        /// <param name="failedReason">The reason for the error being thrown</param>
        /// <param name="property">The property causing the error to be thrown (if any)</param>
        public HttpActionValidationException(HttpStatusCode statusCode, FailedReason failedReason, Property? property = null) : base()
        {
            StatusCode = statusCode;
            FailedReason = failedReason;
            Property = property;
        }
    }
}
