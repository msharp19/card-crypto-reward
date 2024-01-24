using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.API.Helpers
{
    public class OrderHelper
    {
        /// <summary>
        /// Validate the order property
        /// </summary>
        /// <param name="propertyToValidate">The order property to validate</param>
        /// <returns>If the order property is valid</returns>
        public static bool ResolveOrderProperty(string? propertyToValidate, Func<List<(string PropertyName, Order Order)>> function, out string resolvedOrderProperty)
        {
            // Set the resolved property so can return
            resolvedOrderProperty = string.Empty;

            // Get order properties
            var filters = function();

            // Check we even have an order property to check
            if (string.IsNullOrEmpty(propertyToValidate))
            {
                resolvedOrderProperty = filters.FirstOrDefault().PropertyName;
                return !string.IsNullOrEmpty(resolvedOrderProperty);
            }

            // Check if we have a match, if so then return true
            var propertyName = propertyToValidate.ToString();
            var matchingFilter = filters.FirstOrDefault(x => x.PropertyName == propertyName);

            // This is incase its not matched - it will revert to default
            if (!string.IsNullOrEmpty(matchingFilter.PropertyName))
                resolvedOrderProperty = matchingFilter.PropertyName;

            // Check we even have an order property to check
            if (string.IsNullOrEmpty(matchingFilter.PropertyName))
            {
                resolvedOrderProperty = filters.FirstOrDefault().PropertyName;
                return !string.IsNullOrEmpty(resolvedOrderProperty);
            }

            return !string.IsNullOrEmpty(matchingFilter.PropertyName);
        }
    }
}