using System.ComponentModel.DataAnnotations;

namespace GoodHamburger.Api.DTOs;

/// <summary>
/// DTO para atualização de um pedido existente.
/// </summary>
public class UpdateOrderRequest
{
    /// <summary>
    /// Nova lista de IDs dos itens do cardápio para substituir os itens atuais do pedido.
    /// </summary>
    [Required(ErrorMessage = "A lista de itens é obrigatória.")]
    [MinLength(1, ErrorMessage = "O pedido deve conter pelo menos um item.")]
    public List<int> MenuItemIds { get; set; } = new();
}
