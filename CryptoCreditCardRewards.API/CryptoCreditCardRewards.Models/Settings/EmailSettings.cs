using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Settings
{
    /// <summary>
    /// Options for email service
    /// </summary>
    public class EmailSettings
    {
        public string ApplicationUrl { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Enabled { get; set; }
        public string AdminEmailAddress { get; set; }
        public string EmailDisplayName { get; set; }
    }
}
