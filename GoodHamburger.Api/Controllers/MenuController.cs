using GoodHamburger.Api.DTOs;
using GoodHamburger.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers;

/// <summary>
/// Controller para consulta do cardápio da lanchonete.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    /// <summary>
    /// Retorna todos os itens disponíveis no cardápio.
    /// </summary>
    /// <returns>Lista de itens do cardápio.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MenuItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var items = await _menuService.GetAllAsync();
        return Ok(items);
    }

    /// <summary>
    /// Retorna um item específico do cardápio pelo ID.
    /// </summary>
    /// <param name="id">ID do item do cardápio.</param>
    /// <returns>O item do cardápio, se encontrado.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MenuItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _menuService.GetByIdAsync(id);

        if (item is null)
            return NotFound(new { message = $"Item do cardápio com ID '{id}' não foi encontrado." });

        return Ok(item);
    }
}
