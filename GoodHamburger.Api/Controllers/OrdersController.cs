using GoodHamburger.Api.DTOs;
using GoodHamburger.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers;

/// <summary>
/// Controller para gestão de pedidos (CRUD completo).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Lista todos os pedidos.
    /// </summary>
    /// <returns>Lista de pedidos com detalhes.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _orderService.GetAllAsync();
        return Ok(orders);
    }

    /// <summary>
    /// Consulta um pedido pelo ID.
    /// </summary>
    /// <param name="id">ID do pedido (GUID).</param>
    /// <returns>Os detalhes do pedido.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var order = await _orderService.GetByIdAsync(id);
        return Ok(order);
    }

    /// <summary>
    /// Cria um novo pedido.
    /// </summary>
    /// <param name="request">Dados do pedido contendo a lista de IDs dos itens do cardápio.</param>
    /// <returns>O pedido criado com os cálculos de desconto.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        var order = await _orderService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    /// <summary>
    /// Atualiza um pedido existente.
    /// </summary>
    /// <param name="id">ID do pedido a ser atualizado.</param>
    /// <param name="request">Novos dados do pedido.</param>
    /// <returns>O pedido atualizado com os recálculos de desconto.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrderRequest request)
    {
        var order = await _orderService.UpdateAsync(id, request);
        return Ok(order);
    }

    /// <summary>
    /// Remove um pedido pelo ID.
    /// </summary>
    /// <param name="id">ID do pedido a ser removido.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _orderService.DeleteAsync(id);
        return NoContent();
    }
}
