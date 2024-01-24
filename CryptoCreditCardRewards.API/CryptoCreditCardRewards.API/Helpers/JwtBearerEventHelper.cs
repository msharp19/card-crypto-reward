using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace CryptoCreditCardRewards.API.Helpers
{
    public class JwtBearerEventHelper
    {
        /// <summary>
        /// Create JWT bearer events to ensure the role claims are added to identity even if we come from the openID flow
        /// </summary>
        /// <returns>JWT events</returns>
        /// <exception cref="Exception">Thrown if no email claim is found in authenticated identity</exception>
        public static JwtBearerEvents CreateJwtBearerEvents()
        {
            // Create the events
            return new JwtBearerEvents
            {
                // We only care about on token validated
                OnTokenValidated = async ctx =>
                {
                    // Get the callers email - throw error if not provided
                    var email = ctx.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
                    if (string.IsNullOrEmpty(email))
                        throw new Exception("Email is empty when it is expected");

                    // Create the claims
                    var claims = new List<Claim>();

                    // Create a new claims identity
                    var appIdentity = new ClaimsIdentity(claims);

                    // Add the identity to the principle
                    ctx.Principal?.AddIdentity(appIdentity);
                }
            };
        }
    }
}
