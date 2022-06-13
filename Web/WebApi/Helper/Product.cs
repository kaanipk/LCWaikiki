using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Context;

namespace WebApi.Helper
{
    public class Product
    {
        public Product(ApplicationDBContext context)
        {
            _applicationDBContext = context;
        }
        readonly ApplicationDBContext _applicationDBContext;

        public Entities.Product IsExist(string barcode)
        {
            return _applicationDBContext.Products.FirstOrDefault(x => x.Barcode == barcode);
        }

        public bool GetOrderInProductExist(List<Model.OrderLine> orderLine)
        {

            var orderProductCount = orderLine.Count();

            var itemProductCount = _applicationDBContext.Products.Where(x => orderLine.Select(x => x.Barcode).Contains(x.Barcode)).Count();

            if(orderProductCount != itemProductCount)
            {
                //Siparişteki ürünler arasında Product tablosunda olmayan kayıtlar mevcut !
                return false;
            }
            else
            {
                return true;
            }

        }
    }
}
