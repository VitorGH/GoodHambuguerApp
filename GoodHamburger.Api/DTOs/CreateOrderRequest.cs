using System.ComponentModel.DataAnnotations;

namespace GoodHamburger.Api.DTOs;

/// <summary>
/// DTO para criação de um novo pedido.
/// </summary>
public class CreateOrderRequest
{
    /// <summary>
    /// Lista de IDs dos itens do cardápio que compõem o pedido.
    /// </summary>
    [Required(ErrorMessage = "A lista de itens é obrigatória.")]
    [MinLength(1, ErrorMessage = "O pedido deve conter pelo menos um item.")]
    public List<int> MenuItemIds { get; set; } = new();
}
