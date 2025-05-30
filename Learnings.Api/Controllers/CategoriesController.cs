using Learnings.Application.Dtos.ProductsDto;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Learnings.Api.Controllers
{
    //[Authorize(Roles = "SuperAdmin")]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _svc;
        public CategoriesController(ICategoryService svc) => _svc = svc;

        [AllowAnonymous]
        [HttpGet("AllParentCategories")]
        public async Task<ActionResult<ResponseBase<List<CategoryDto>>>> GetAllParentCategories()
        {
            var response = await _svc.GetAllParentCategoriesAsync();
            if (response.Data == null)
                return NotFound(new ResponseBase<string>(null, response.Message, response.Status));
            return Ok(response);
        }
        [HttpGet]
        public async Task<ActionResult<ResponseBase<List<CategoryDto>>>> GetAll()
        {
            var response = await _svc.GetAllAsync();
            if (response.Data == null)
                return NotFound(new ResponseBase<string>(null, response.Message, response.Status));
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseBase<CategoryDto>>> GetById([FromRoute]Guid id)
        {
            var response = await _svc.GetByIdAsync(id);
            if (response.Data == null)
                return NotFound(new ResponseBase<string>(null, response.Message, response.Status));
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ResponseBase<CategoryDto>>> Create([FromBody] CategoryDto dto)
        {
            var response = await _svc.CreateAsync(dto);
            return StatusCode((int)response.Status, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseBase<CategoryDto>>> Update(Guid id, [FromBody] CategoryDto dto)
        {
            var response = await _svc.UpdateAsync(id, dto);
            if (response.Data == null)
                return StatusCode((int)response.Status, response);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseBase<string>>> Delete(Guid id)
        {
            var response = await _svc.DeleteAsync(id);
            if (response.Data == null)
                return StatusCode((int)response.Status, response);
            return Ok(response);
        }
    }
}
