namespace GoodHamburger.Api.Models;

/// <summary>
/// Representa um item dentro de um pedido, associando o pedido ao item do cardápio.
/// </summary>
public class OrderItem
{
    public int Id { get; set; }
    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int MenuItemId { get; set; }
    public MenuItem MenuItem { get; set; } = null!;
}
