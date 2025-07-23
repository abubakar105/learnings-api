using Learnings.Application.Dtos.RolesDto;
using Learnings.Application.ResponseBase;
using Learnings.Application.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly IHttpContextAccessor _httpContext;

    public CartController(ICartService cartService, IHttpContextAccessor httpContext)
    {
        _cartService = cartService;
        _httpContext = httpContext;
    }

    private string UserId => _httpContext.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

    [HttpGet]
    public async Task<ActionResult<ResponseBase<CartDto>>> GetCart()
    {
        var resp = await _cartService.GetCartByUserAsync(UserId);
        return Ok(resp);
    }

    [HttpPost("items")]
    public async Task<ActionResult<ResponseBase<CartDto>>> AddItem([FromBody] AddCartItemRequest req)
    {
        var resp = await _cartService.AddItemAsync(UserId, req);
        return StatusCode((int)resp.Status, resp);
    }

    [HttpPut("items")]
    public async Task<ActionResult<ResponseBase<CartDto>>> UpdateItem([FromBody] UpdateCartItemRequest req)
    {
        var resp = await _cartService.UpdateItemAsync(UserId, req);
        return StatusCode((int)resp.Status, resp);
    }

    [HttpDelete("items/{id:guid}")]
    public async Task<ActionResult<ResponseBase<string>>> RemoveItem(Guid id)
    {
        var resp = await _cartService.RemoveItemAsync(UserId, id);
        return StatusCode((int)resp.Status, resp);
    }

    [HttpDelete]
    public async Task<ActionResult<ResponseBase<string>>> ClearCart()
    {
        var resp = await _cartService.ClearCartAsync(UserId);
        return StatusCode((int)resp.Status, resp);
    }
}
