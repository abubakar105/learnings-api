using Learnings.Api.Hubs;
using Learnings.Application.Dtos.ProductsDto;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.Interface;
using Learnings.Application.Services.Interface.Contracts.HUBS;
using Learnings.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Org.BouncyCastle.Crypto;

namespace Learnings.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _svc;
        public ReviewsController(IReviewService svc) => _svc = svc;

        [HttpPost]
        public async Task<ActionResult<ResponseBase<ReviewDto>>> Create([FromBody] AddReviewDto dto)
        {
            var resp = await _svc.CreateReviewAsync(dto);
            return StatusCode((int)resp.Status, resp);
        }

        [HttpGet("getAllProductReviews/{productId}")]
        public async Task<ActionResult<ResponseBase<List<ReviewDto>>>> GetAll([FromRoute] Guid productId)
        {
            var resp = await _svc.GetAllReviewsAsync(productId);
            return StatusCode((int)resp.Status, resp);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseBase<ReviewDto>>> GetById(Guid id)
        {
            var resp = await _svc.GetReviewByIdAsync(id);
            return StatusCode((int)resp.Status, resp);
        }

        [HttpPut]
        public async Task<ActionResult<ResponseBase<ReviewDto>>> Update([FromBody] UpdateReviewDto dto)
        {
            var resp = await _svc.UpdateReviewAsync(dto);
            return StatusCode((int)resp.Status, resp);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseBase<bool>>> Delete(Guid id)
        {
            var resp = await _svc.DeleteReviewAsync(id);
            return StatusCode((int)resp.Status, resp);
        }
    }
}
