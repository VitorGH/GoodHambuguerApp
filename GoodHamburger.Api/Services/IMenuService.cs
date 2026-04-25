using GoodHamburger.Api.DTOs;
using GoodHamburger.Api.Models;

namespace GoodHamburger.Api.Services;

/// <summary>
/// Interface para o serviço de cardápio.
/// </summary>
public interface IMenuService
{
    /// <summary>
    /// Retorna todos os itens do cardápio.
    /// </summary>
    Task<IEnumerable<MenuItemResponse>> GetAllAsync();

    /// <summary>
    /// Retorna um item do cardápio por ID.
    /// </summary>
    Task<MenuItemResponse?> GetByIdAsync(int id);
}
