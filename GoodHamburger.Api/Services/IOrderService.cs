using GoodHamburger.Api.DTOs;

namespace GoodHamburger.Api.Services;

/// <summary>
/// Interface para o serviço de pedidos.
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Retorna todos os pedidos.
    /// </summary>
    Task<IEnumerable<OrderResponse>> GetAllAsync();

    /// <summary>
    /// Retorna um pedido pelo ID.
    /// </summary>
    Task<OrderResponse> GetByIdAsync(Guid id);

    /// <summary>
    /// Cria um novo pedido.
    /// </summary>
    Task<OrderResponse> CreateAsync(CreateOrderRequest request);

    /// <summary>
    /// Atualiza um pedido existente.
    /// </summary>
    Task<OrderResponse> UpdateAsync(Guid id, UpdateOrderRequest request);

    /// <summary>
    /// Remove um pedido pelo ID.
    /// </summary>
    Task DeleteAsync(Guid id);
}
