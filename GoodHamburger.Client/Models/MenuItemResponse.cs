namespace GoodHamburger.Client.Models;

/// <summary>
/// DTO de resposta de um item do cardápio (espelha o DTO da API).
/// </summary>
public class MenuItemResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
