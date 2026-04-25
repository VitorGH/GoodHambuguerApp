namespace GoodHamburger.Api.Models;

/// <summary>
/// Representa um item do cardápio da lanchonete.
/// </summary>
public class MenuItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public MenuItemType Type { get; set; }
    public decimal Price { get; set; }
}
