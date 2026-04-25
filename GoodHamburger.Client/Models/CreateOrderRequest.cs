namespace GoodHamburger.Client.Models;

/// <summary>
/// DTO para criação de um novo pedido (espelha o DTO da API).
/// </summary>
public class CreateOrderRequest
{
    public List<int> MenuItemIds { get; set; } = new();
}
