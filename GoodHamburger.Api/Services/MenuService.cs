using GoodHamburger.Api.Data;
using GoodHamburger.Api.DTOs;
using GoodHamburger.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Api.Services;

/// <summary>
/// Serviço responsável por consultar os itens do cardápio.
/// </summary>
public class MenuService : IMenuService
{
    private readonly AppDbContext _context;

    public MenuService(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<MenuItemResponse>> GetAllAsync()
    {
        var items = await _context.MenuItems
            .AsNoTracking()
            .OrderBy(m => m.Type)
            .ThenBy(m => m.Name)
            .ToListAsync();

        return items.Select(MapToResponse);
    }

    /// <inheritdoc />
    public async Task<MenuItemResponse?> GetByIdAsync(int id)
    {
        var item = await _context.MenuItems
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        return item is null ? null : MapToResponse(item);
    }

    private static MenuItemResponse MapToResponse(MenuItem item) => new()
    {
        Id = item.Id,
        Name = item.Name,
        Type = item.Type.ToString(),
        Price = item.Price
    };
}
