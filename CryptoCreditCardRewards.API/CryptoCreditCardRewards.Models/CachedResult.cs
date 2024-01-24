﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoCreditCardRewards.Models
{
    public class CachedResult<T>
    {
        public DateTime DateCached { get; set; }
        public T Item { get; set; }
    }
}
