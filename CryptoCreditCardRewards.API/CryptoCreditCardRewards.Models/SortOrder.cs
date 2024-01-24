using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Models
{
    public class SortOrder
    {
        public string OrderProperty { get; set; }
        public Order Order { get; set; }

        public SortOrder(string orderProperty, Order order)
        {
            OrderProperty = orderProperty;
            Order = order;
        }
    }
}
