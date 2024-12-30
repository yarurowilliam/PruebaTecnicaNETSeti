using Microsoft.AspNetCore.Mvc;
using PruebaTecnicaNET.Models;
using PruebaTecnicaNET.Services;

namespace PruebaTecnicaNET.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<ActionResult<EnviarPedidoRespuesta>> ProcessOrder([FromBody] EnviarPedido request)
    {
        if (request?.EnviarPedidoRequest == null)
            return BadRequest("Invalid request format");

        var response = await _orderService.ProcessOrderAsync(request);
        return Ok(response);
    }
}
