using Learnings.Application.Dtos.ProductsDto;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Learnings.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]     
    //[Authorize(Roles = "SuperAdmin")]
    public class ProductsOdataController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsOdataController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpPost]
        public async Task<ActionResult<ResponseBase<AddProductDto>>> Create([FromBody] AddProductDto dto)
        {
            var response = await _productService.CreateProduct(dto);
            return StatusCode((int)response.Status, response);
        }
        [HttpGet]
        [EnableQuery]                   // ← this lets OData apply $filter, $top, $skip, $orderby, $count, etc.
        public IQueryable<AddProductDto> Get()
        {
            return _productService.GetProducts();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseBase<AddProductDto>>> GetProductById([FromRoute(Name = "id")] Guid productId)
        {
            var response = await _productService.GetSingleProduct(productId);
            return StatusCode((int)response.Status, response);
        }
    }
}
