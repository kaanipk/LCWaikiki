using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using WebApi.Context;
using WebApi.Helper;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/customer")]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        public CustomerController(ApplicationDBContext context, ILogger<CustomerController> logger)
        {
            _logger = logger;
            _applicationDBContext = context;
        }
        readonly ApplicationDBContext _applicationDBContext;
        [HttpPost("AddOrUpdateCustomer")]
        public ActionResult AddOrUpdateCustomer([FromBody] Model.Customer customer)
        {
            try
            {
                _logger.LogInformation($"#Customer #AddOrUpdateCustomer #Request: {JsonConvert.SerializeObject(customer)}");

                if (customer.CustomerId != 0)
                {
                    //Update
                    Customer customerHelper = new Customer(_applicationDBContext);
                    var entities = customerHelper.IsExist(customer.CustomerId);

                    if (entities == null)
                    {
                        _logger.LogWarning($"#Customer #AddOrUpdateCustomer #Request: {JsonConvert.SerializeObject(customer)} #Response: Customer Not Found !");
                        return NotFound("Customer Not Found !");
                    }
                    else
                    {
                        entities.Name = customer.Name;

                        entities.Address = customer.Address;
                        entities.LastUpdatedDate = DateTime.Now;

                        _applicationDBContext.Update(entities);
                        _applicationDBContext.SaveChanges();

                        _logger.LogInformation($"#Customer #AddOrUpdateCustomer #Request: {JsonConvert.SerializeObject(customer)} #Response: OK");
                            
                        return Ok(entities);
                    }
                }
                else
                {
                    //Create
                    var entities = new Entities.Customer
                    {
                        Name = customer.Name,
                        Address = customer.Address,
                        CreatedDate = DateTime.Now,
                        LastUpdatedDate = DateTime.Now
                    };

                    _applicationDBContext.Add(entities);
                    _applicationDBContext.SaveChanges();

                    _logger.LogInformation($"#Customer #AddOrUpdateCustomer #Request: {JsonConvert.SerializeObject(customer)} #Response: OK");
                    return Ok(entities);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"#Customer #AddOrUpdateCustomer #Request: {JsonConvert.SerializeObject(customer)} #Response: {ex.Message.ToString()}");
                return Problem(ex.Message.ToString());
            }
        }
        [HttpGet("GetCustomer")]
        public ActionResult GetCustomer(long customerId)
        {
            try
            {
                using (_applicationDBContext)
                {
                    _logger.LogInformation($"#Customer #GetCustomer #Request: {customerId}");

                    Customer customer = new Customer(_applicationDBContext);
                    var entities = customer.IsExist(customerId);
                    
                    if (entities == null)
                    {
                        _logger.LogInformation($"#Customer #GetCustomer #Request: {customerId} #Response: Customer Not Found !" );
                        return NotFound("Customer Not Found !");
                    }
                    else
                    {
                        _logger.LogInformation($"#Customer #GetCustomer #Request: {customerId} #Response:{JsonConvert.SerializeObject(entities)}!");

                        return Ok(entities);
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"#Customer #GetCustomer #Request: {customerId} #Response: {ex.Message}");
                return Problem(ex.Message.ToString());
            }
        }
        [HttpDelete("DeleteCustomer")]
        public ActionResult DeleteCustomer([FromBody] long customerId)
        {
            try
            {
                using (_applicationDBContext)
                {
                    _logger.LogInformation($"#Customer #DeleteCustomer #Request: {customerId}");

                    Customer customer = new Customer(_applicationDBContext);
                    var entities = customer.IsExist(customerId);

                    if (entities == null)
                    {
                        _logger.LogWarning($"#Customer #DeleteCustomer #Request: {customerId} #Response: Customer Not Found !");
                        return NotFound("Customer Not Found !");
                    }
                    else
                    {
                        _applicationDBContext.Remove(entities);
                        _applicationDBContext.SaveChanges();

                        _logger.LogInformation($"#Customer #DeleteCustomer #Request: {customerId} #Response: Ok !");
                        return Ok();
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"#Customer #DeleteCustomer #Request: {customerId} #Response: {ex.Message}");
                return Problem(ex.Message.ToString());
            }
        }

    }
}
