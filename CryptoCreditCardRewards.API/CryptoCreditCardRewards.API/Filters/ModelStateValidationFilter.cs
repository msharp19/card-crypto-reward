using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace CryptoCreditCardRewards.API.Filters
{
    /// <summary>
    /// Filter to check for modelstate and validate
    /// </summary>
    public class ModelStateValidationFilter : ActionFilterAttribute
    {
        /// <summary>
        /// Called on controller action being executed
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Check if model is valid - otherwise throw bad request before we even hit a method
            if (!context.ModelState.IsValid)
            {
                context.Result = new ContentResult() { Content = "Model is not valid", StatusCode = (int)HttpStatusCode.BadRequest };
                return;
            }

            // Continue as normal if valid
            base.OnActionExecuting(context);
        }
    }
}
