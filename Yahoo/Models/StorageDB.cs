using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yahoo.Models
{
    public class StorageDB
    {
        public int ID { get; set; }
        public string symbol { get; set; }
        public string CompanyName { get; set; }
        public DateTime yearFounded { get; set; }
        public int numberOfEmployees { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string MarketCap { get; set; }
    }
}
