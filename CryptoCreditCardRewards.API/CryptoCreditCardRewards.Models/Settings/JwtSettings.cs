using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Settings
{
    /// <summary>
    /// Options for JWT generation/management
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// The issuer of the token (authority)
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// The audience of the token (authority)
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// The key used to sign token upon creation and to read when requested
        /// </summary>
        public string JwtSigningKey { get; set; }

        /// <summary>
        /// When the jwt expires
        /// </summary>
        public string Expiry { get; set; }

        /// <summary>
        /// When the refresh token expires
        /// </summary>
        public string RefreshTokenExpiry { get; set; }

        /// <summary>
        /// Audiences to validate against - this should include any audience specified above
        /// </summary>
        public List<string> ValidAudiences { get; set; } = new List<string>();

        /// <summary>
        /// Issuers to validate against - this should include any issuer specified above
        /// </summary>
        public List<string> ValidIssuers { get; set; } = new List<string>();
    }
}
