using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Dtos.Accounts
{
    public class CreateUserDto
    {
        /// <summary>
        /// The users email to register
        /// </summary>
        [RegularExpression(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,50})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$")]
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// The users account number to register
        /// </summary>
        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; }

        /// <summary>
        /// If the user has completed KYC (blocker for withdraw)
        /// </summary>
        [JsonProperty("completedKyc")]
        public bool CompletedKyc { get; set; }

        /// <summary>
        /// The created date
        /// </summary>
        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }
    }
}
