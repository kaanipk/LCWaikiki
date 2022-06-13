using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Context;

namespace WebApi.Helper
{
    public class Order
    {
        public Order(ApplicationDBContext context)
        {
            _applicationDBContext = context;
        }
        readonly ApplicationDBContext _applicationDBContext;

        public Entities.Order IsExist(int orderId)
        {
            return _applicationDBContext.Orders.Include(x=>x.OrderLines).FirstOrDefault(x => x.OrderId == orderId);
        }
    }
}
