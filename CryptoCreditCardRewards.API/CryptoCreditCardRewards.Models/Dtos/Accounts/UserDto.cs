using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Dtos.Accounts
{
    public class UserDto
    {
        /// <summary>
        /// The users id
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// The users email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// The users account number
        /// </summary>
        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; }

        /// <summary>
        /// When the user has completed kyc (if at all)
        /// </summary>
        [JsonProperty("completedKycDate")]
        public DateTime? CompletedKycDate { get; set; }

        /// <summary>
        /// The created date
        /// </summary>
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }
    }
}
