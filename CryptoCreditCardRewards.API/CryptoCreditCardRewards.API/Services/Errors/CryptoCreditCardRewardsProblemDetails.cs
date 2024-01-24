using Microsoft.AspNetCore.Mvc;

namespace CryptoCreditCardRewards.API.Services.Errors
{
    public class CryptoCreditCardRewardsProblemDetails : ProblemDetails
    {
        public string TraceId { get; set; }
        public string Stacktrace { get; set; }
        public object Errors { get; set; }
    }
}
