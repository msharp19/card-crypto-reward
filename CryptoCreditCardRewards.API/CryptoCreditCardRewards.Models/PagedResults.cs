using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models
{
    public class PagedResults<T>
    {
        public List<T> Items { get; set; }
        public Page Page { get; set; }
        public SortOrder SortOrder { get; set; }
        public int TotalCount { get; set; }
    }
}
