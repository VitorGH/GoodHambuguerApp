namespace GoodHamburger.Client.Models;

/// <summary>
/// DTO de resposta de um pedido (espelha o DTO da API).
/// </summary>
public class OrderResponse
{
    public Guid Id { get; set; }
    public List<OrderItemResponse> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO de resposta de um item dentro de um pedido.
/// </summary>
public class OrderItemResponse
{
    public int MenuItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
