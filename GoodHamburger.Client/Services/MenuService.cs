using System.Net.Http.Json;
using GoodHamburger.Client.Models;

namespace GoodHamburger.Client.Services;

/// <summary>
/// Serviço HTTP para consumir os endpoints de cardápio da API.
/// </summary>
public class MenuService
{
    private readonly HttpClient _http;

    public MenuService(HttpClient http)
    {
        _http = http;
    }

    /// <summary>
    /// Retorna todos os itens do cardápio.
    /// </summary>
    public async Task<List<MenuItemResponse>> GetAllAsync()
    {
        var items = await _http.GetFromJsonAsync<List<MenuItemResponse>>("api/menu");
        return items ?? new List<MenuItemResponse>();
    }
}
