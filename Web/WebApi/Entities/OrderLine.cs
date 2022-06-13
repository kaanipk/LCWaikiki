using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Entities
{
    public class OrderLine
    {
        public int OrderLineId { get; set; }
        public virtual Order Orders { get; set; }
        public int OrderId { get; set; }
        public string Barcode { get; set; }
        public int Quantity { get; set; }
    }
}
