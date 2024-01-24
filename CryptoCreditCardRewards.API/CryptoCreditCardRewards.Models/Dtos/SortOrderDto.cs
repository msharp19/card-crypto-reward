using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoCreditCardRewards.Models.Enums;

namespace CryptoCreditCardRewards.Models.Dtos
{
    public class SortOrderDto
    {
        public string OrderProperty { get; set; }
        public Order Order { get; set; }
    }
}
