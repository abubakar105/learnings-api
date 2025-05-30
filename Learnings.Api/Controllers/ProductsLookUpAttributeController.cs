using Learnings.Application.Dtos.ProductsDto;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Learnings.Api.Controllers
{
    //[Authorize(Roles = "SuperAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsLookUpAttributeController : ControllerBase
    {
        private readonly IProductsLookUpAttributeService _service;

        public ProductsLookUpAttributeController(IProductsLookUpAttributeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseBase<List<ProductsLookUpAttributeDto>>>> GetAll()
        {
            var response = await _service.GetAllAsync();
            if (response.Data == null)
                return NotFound(new ResponseBase<string>(null, response.Message, response.Status));

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseBase<ProductsLookUpAttributeDto>>> GetById(Guid id)
        {
            var response = await _service.GetByIdAsync(id);
            if (response.Data == null)
                return NotFound(new ResponseBase<string>(null, response.Message, response.Status));

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ResponseBase<ProductsLookUpAttributeDto>>> Create([FromBody] ProductsLookUpAttributeDto dto)
        {
            var response = await _service.CreateAsync(dto);
            return StatusCode((int)response.Status, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseBase<ProductsLookUpAttributeDto>>> Update(Guid id, [FromBody] ProductsLookUpAttributeDto dto)
        {
            var response = await _service.UpdateAsync(id, dto);
            if (response.Data == null)
                return StatusCode((int)response.Status, response);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseBase<string>>> Delete(Guid id)
        {
            var response = await _service.DeleteAsync(id);
            if (response.Data == null)
                return StatusCode((int)response.Status, response);

            return Ok(response);
        }
        [HttpPost("lookupValue")]
        public async Task<ActionResult<ResponseBase<List<ProductsLookUpAttributesValueDto>>>> GetValuesOfLookupAttribute([FromBody] LookupRequestDto request)
        {
            var response = await _service.GetValuesOfLookupAttribute(request);
            if (response.Data == null)
                return NotFound(new ResponseBase<string>(null, response.Message, response.Status));

            return Ok(response);
        }
    }
}
