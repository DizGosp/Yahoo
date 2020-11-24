using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yahoo.ViewModel
{
    public class StorageVM
    {
        public List<Rows> podaci { get; set; }

        public class Rows
        {
            public string symbol { get; set; }
            public string CompanyName { get; set; }
            public DateTime yearFounded { get; set; }
            public int numberOfEmployees { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public decimal OpenPrice { get; set; }
            public decimal ClosePrice { get; set; }
            public string MarketCap { get; set; }
            public DateTime Datum { get; set; }
        }
        
    }

    
}
