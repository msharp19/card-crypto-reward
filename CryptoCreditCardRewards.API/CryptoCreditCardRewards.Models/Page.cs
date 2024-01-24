using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models
{
    public class Page
    {
        public int PageIndex { get; set; }
        public int PerPage { get; set; }

        public Page(int pageIndex, int perPage)
        {
            PageIndex = pageIndex;
            PerPage = perPage;
        }
    }
}
