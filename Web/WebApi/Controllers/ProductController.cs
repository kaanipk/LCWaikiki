using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json;
using System;
using System.Linq;
using WebApi.Context;


namespace WebApi.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<CustomerController> _logger;
        public ProductController(ApplicationDBContext context, ILogger<CustomerController> logger, IElasticClient elasticClient)
        {
            _logger = logger;
            _applicationDBContext = context;
            _elasticClient = elasticClient;
        }
        readonly ApplicationDBContext _applicationDBContext;

        [HttpGet("GetProduct")]
        public ActionResult GetProduct()
        {
            try
            {
                _logger.LogInformation($"#Product #GetProduct");
                var searchResponse = _elasticClient.Search<Model.Product>(s => s.Query(q => q.MatchAll()));
                _logger.LogInformation($"#Product #GetProduct #response: {JsonConvert.SerializeObject(searchResponse)}");
                return Ok(searchResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError($"#Product #GetProduct #response: {JsonConvert.SerializeObject(ex.Message)}");
                return Problem(ex.Message);
            }

        }

        [HttpPost("AddOrUpdateProduct")]
        public ActionResult AddOrUpdateProduct(Model.Product product)
        {
            try
            {
                _logger.LogInformation($"#Product #AddOrUpdateProduct #Request: {JsonConvert.SerializeObject(product)}");


                Helper.Product productHelper = new Helper.Product(_applicationDBContext);
                var entities = productHelper.IsExist(product.Barcode);


                if (entities == null)
                {
                    //Create

                     entities = new Entities.Product
                    {
                        Barcode = product.Barcode,
                        CreatedDate = DateTime.Now,
                        LastUpdatedDate= DateTime.Now,
                        Description = product.Description,
                        Price = product.Price
                    };

                    _applicationDBContext.Add(entities);
                    _applicationDBContext.SaveChanges();

                    _logger.LogInformation($"#Product #AddOrUpdateProduct #Request: {JsonConvert.SerializeObject(product)}  #Response: {JsonConvert.SerializeObject(entities)}");

                    return Ok(entities);
                }
                else
                {

                    //Update

                    entities.Price = product.Price;
                    entities.Description = product.Description;
                    entities.LastUpdatedDate = product.LastUpdatedDate;
                    _applicationDBContext.Update(entities);
                    _applicationDBContext.SaveChanges();

                    _logger.LogInformation($"#Product #AddOrUpdateProduct #Request: {JsonConvert.SerializeObject(product)}  #Response: {JsonConvert.SerializeObject(entities)}");
                    return Ok(entities);
                }



            }
            catch (Exception ex)
            {
                _logger.LogError($"#Product #AddOrUpdateProduct #Request: {JsonConvert.SerializeObject(product)} #Response: {ex.Message}");
                return Problem(ex.Message.ToString());
            }
        }


    }
}
