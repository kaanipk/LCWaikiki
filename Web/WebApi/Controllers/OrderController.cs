using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Context;
using WebApi.Helper;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        public OrderController(ApplicationDBContext context, ILogger<CustomerController> logger)
        {
            _logger = logger;
            _applicationDBContext = context;
        }
        readonly ApplicationDBContext _applicationDBContext;

        [HttpPost("AddOrUpdateOrder")]
        public ActionResult AddOrUpdateOrder([FromBody] WebApi.Model.Order order)
        {
            try
            {
                _logger.LogInformation($"#Order #AddOrUpdateOrder #Request: {JsonConvert.SerializeObject(order)}");

                Product product = new Product(_applicationDBContext);

                if (!product.GetOrderInProductExist(order.OrderLines)) 
                {
                    _logger.LogWarning($"#Order #AddOrUpdateOrder #Request: {JsonConvert.SerializeObject(order)} #Response: Products Not Found !");
                    return Problem("Products Not Found !");
                }


                if (order.OrderId != 0)
                {
                    //Update

                    Order orderHelper = new Order(_applicationDBContext);
                    var entities = orderHelper.IsExist(order.OrderId);

                    if (entities == null)
                    {

                        _logger.LogWarning($"#Order #AddOrUpdateOrder #Request: {JsonConvert.SerializeObject(order)} #Response: Order Not Found !");
                        return Problem("Order Not Found !");
                    }
                    else
                    {
                        entities.LastUpdatedDate = DateTime.Now;
                        entities.CustomerId = order.CustomerId;
                        entities.OrderLines = order.OrderLines.Select(x => new Entities.OrderLine { Barcode = x.Barcode, Quantity = x.Quantity }).ToList();
                        _applicationDBContext.Update(entities);
                        _applicationDBContext.SaveChanges();

                        _logger.LogInformation($"#Order #AddOrUpdateOrder #Request: {JsonConvert.SerializeObject(order)} #Response: Ok");

                        return Ok(entities);
                    }
                }
                else
                {
                    //Create

                    // Bu siparişe ait bir müşteri kaydı içeride var mı ?
                    Customer customer = new Customer(_applicationDBContext);

                    if (customer.IsExist(order.CustomerId) == null)
                    {
                        _logger.LogWarning($"#Order #AddOrUpdateOrder #Request: {JsonConvert.SerializeObject(order)} #Response: Customer Not Found !");
                        return Problem("Customer Not Found !");
                    }


                    using (_applicationDBContext)
                    {
                        var entities = new WebApi.Entities.Order
                        {
                            CustomerId = order.CustomerId,
                            OrderLines = order.OrderLines.Select(x => new Entities.OrderLine { Barcode = x.Barcode, Quantity = x.Quantity }).ToList(),
                            CreatedDate = DateTime.Now,
                            LastUpdatedDate = DateTime.Now
                        };

                        _applicationDBContext.Add(entities);
                        _applicationDBContext.SaveChanges();


                        _logger.LogInformation($"#Order #AddOrUpdateOrder #Request: {JsonConvert.SerializeObject(order)} #Response: Ok");
                        return Ok(entities);

                    }
                }

               
            }
            catch (Exception ex)
            {
                _logger.LogError($"#Order #AddOrUpdateOrder #Request: {JsonConvert.SerializeObject(order)} #Response: {ex.Message}");
                return Problem(ex.Message.ToString());
            }



        }

        [HttpDelete("DeleteOrder")]
        public ActionResult DeleteOrder([FromBody] int orderId)
        {
            try
            {
                _logger.LogInformation($"#Order #DeleteOrder #Request: {orderId}");
                Order order = new Order(_applicationDBContext);
                var entities = order.IsExist(orderId);

                if (entities == null)
                {
                    _logger.LogWarning($"#Order #DeleteOrder #Request: {orderId} #Response: Order Not Found !");
                    return NotFound("Order Not Found ! ");
                }

                using (_applicationDBContext)
                {

                    _applicationDBContext.Orders.Remove(entities);
                    _applicationDBContext.SaveChanges();

                    _logger.LogInformation($"#Order #DeleteOrder #Request: {orderId} #Response: OK");
                    return Ok();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"#Order #DeleteOrder #Request: {orderId} #Response: {ex.Message}");
                return Problem(ex.Message.ToString());

            }
        }

        [HttpGet("GetOrder")]
        public ActionResult GetOrder(int orderId)
        {
            try
            {
                _logger.LogInformation($"#Order #GetOrder #Request: {orderId}");
                using (_applicationDBContext)
                {
                    Order order = new Order(_applicationDBContext);
                    var entities = order.IsExist(orderId);


                    if (entities == null)
                    {
                        _logger.LogWarning($"#Order #GetOrder #Request: {orderId} #Response: Order Not Found !");
                        return NotFound("Order Not Found !");
                    }
                    else
                    {

                        _logger.LogInformation($"#Order #GetOrder #Request: {orderId} #Response: OK");
                        return Ok(entities);
                    }


                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message.ToString());
            }

        }
    }
}
