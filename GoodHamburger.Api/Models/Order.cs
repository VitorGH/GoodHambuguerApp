namespace GoodHamburger.Api.Models;

/// <summary>
/// Representa um pedido realizado na lanchonete.
/// </summary>
public class Order
{
    public Guid Id { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
