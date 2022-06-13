using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Context;

namespace WebApi.Helper
{
    public class Customer
    {
        public Customer(ApplicationDBContext context)
        {
            _applicationDBContext = context;
        }
        readonly ApplicationDBContext _applicationDBContext;

        public Entities.Customer IsExist(long customerId)
        {
            return _applicationDBContext.Customers.FirstOrDefault(x => x.CustomerId == customerId);
        }

    }
}
