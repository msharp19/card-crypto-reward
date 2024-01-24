using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Settings
{
    public class OpenIdSettings
    {
        /// <summary>
        /// OpenID configuration endpoints
        /// </summary>
        public List<string> WellKnownEndpoints { get; set; } = new List<string>();

        #region Microsoft Azure AD

        /// <summary>
        /// Issuer pattern for Microsoft Azure AD
        /// </summary>
        public string MicrosoftTenantIssuerPattern { get; set; }

        /// <summary>
        /// The base Token URL for Microsoft Azure AD
        /// </summary>
        public string MicrosoftBaseUrl { get; set; }

        /// <summary>
        /// The client ID for Microsoft Azure AD
        /// </summary>
        public string MicrosoftClientId { get; set; }

        /// <summary>
        /// The client secret for Microsoft Azure AD
        /// </summary>
        public string MicrosoftClientSecret { get; set; }

        #endregion
    }
}
