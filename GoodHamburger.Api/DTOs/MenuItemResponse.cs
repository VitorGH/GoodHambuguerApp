namespace GoodHamburger.Api.DTOs;

/// <summary>
/// DTO de resposta para um item do cardápio.
/// </summary>
public class MenuItemResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
