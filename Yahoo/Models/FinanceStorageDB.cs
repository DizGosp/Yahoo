using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Yahoo.Models
{
    public class FinanceStorageDB
    {
        public int id { get; set; }
        public decimal OpenPrice { get; set; }
        public decimal ClosePrice { get; set; } 
        public DateTime Datum { get; set; }

        public int StorageDBId { get; set; }
        [ForeignKey("StorageDBId")]
        public StorageDB StorageDB { get; set; }
    }
}
