using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models.Settings
{
    public abstract class BaseHostedServiceSettings
    {
        public bool Enabled { get; set; }
        public string Interval { get; set; }
    }
}
