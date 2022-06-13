using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Model
{
    public class Order
    {
        public int OrderId { get; set; }
        public long CustomerId { get; set; }
        public List<OrderLine> OrderLines{ get; set; }

    }

    public class OrderLine
    {
        public string Barcode { get; set; }
        public int Quantity { get; set; }
    }
}
