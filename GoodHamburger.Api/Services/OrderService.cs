using GoodHamburger.Api.Data;
using GoodHamburger.Api.DTOs;
using GoodHamburger.Api.Exceptions;
using GoodHamburger.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Api.Services;

/// <summary>
/// Serviço responsável pela lógica de negócio dos pedidos,
/// incluindo validações, cálculo de descontos e operações CRUD.
/// </summary>
public class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<OrderResponse>> GetAllAsync()
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
                .ThenInclude(oi => oi.MenuItem)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(MapToResponse);
    }

    /// <inheritdoc />
    public async Task<OrderResponse> GetByIdAsync(Guid id)
    {
        var order = await GetOrderWithItemsAsync(id);
        return MapToResponse(order);
    }

    /// <inheritdoc />
    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request)
    {
        // Validar itens duplicados
        ValidateDuplicateItems(request.MenuItemIds);

        // Buscar os itens do cardápio
        var menuItems = await GetMenuItemsAsync(request.MenuItemIds);

        // Validar regras de negócio (quantidade máxima por tipo)
        ValidateItemQuantities(menuItems);

        // Calcular valores
        var subtotal = menuItems.Sum(m => m.Price);
        var discountPercentage = CalculateDiscount(menuItems);
        var discountAmount = subtotal * (discountPercentage / 100m);
        var total = subtotal - discountAmount;

        // Criar o pedido
        var order = new Order
        {
            Id = Guid.NewGuid(),
            Subtotal = subtotal,
            DiscountPercentage = discountPercentage,
            DiscountAmount = Math.Round(discountAmount, 2),
            Total = Math.Round(total, 2),
            CreatedAt = DateTime.UtcNow,
            Items = menuItems.Select(m => new OrderItem
            {
                MenuItemId = m.Id
            }).ToList()
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Recarregar com includes para o response
        var savedOrder = await GetOrderWithItemsAsync(order.Id);
        return MapToResponse(savedOrder);
    }

    /// <inheritdoc />
    public async Task<OrderResponse> UpdateAsync(Guid id, UpdateOrderRequest request)
    {
        var order = await GetOrderWithItemsAsync(id);

        // Validar itens duplicados
        ValidateDuplicateItems(request.MenuItemIds);

        // Buscar os novos itens do cardápio
        var menuItems = await GetMenuItemsAsync(request.MenuItemIds);

        // Validar regras de negócio
        ValidateItemQuantities(menuItems);

        // Recalcular valores
        var subtotal = menuItems.Sum(m => m.Price);
        var discountPercentage = CalculateDiscount(menuItems);
        var discountAmount = subtotal * (discountPercentage / 100m);
        var total = subtotal - discountAmount;

        // Remover itens antigos e adicionar os novos
        _context.OrderItems.RemoveRange(order.Items);

        order.Subtotal = subtotal;
        order.DiscountPercentage = discountPercentage;
        order.DiscountAmount = Math.Round(discountAmount, 2);
        order.Total = Math.Round(total, 2);
        order.UpdatedAt = DateTime.UtcNow;
        order.Items = menuItems.Select(m => new OrderItem
        {
            MenuItemId = m.Id,
            OrderId = order.Id
        }).ToList();

        await _context.SaveChangesAsync();

        // Recarregar com includes para o response
        var updatedOrder = await GetOrderWithItemsAsync(order.Id);
        return MapToResponse(updatedOrder);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        var order = await _context.Orders.FindAsync(id)
            ?? throw new OrderNotFoundException(id);

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
    }

    #region Métodos Privados

    /// <summary>
    /// Busca um pedido com seus itens e itens do cardápio inclusos.
    /// </summary>
    private async Task<Order> GetOrderWithItemsAsync(Guid id)
    {
        return await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(oi => oi.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == id)
            ?? throw new OrderNotFoundException(id);
    }

    /// <summary>
    /// Valida se há itens duplicados na lista de IDs.
    /// </summary>
    private static void ValidateDuplicateItems(List<int> menuItemIds)
    {
        var duplicates = menuItemIds
            .GroupBy(id => id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates.Count > 0)
        {
            throw new DuplicateItemException(
                $"O pedido contém itens duplicados. IDs duplicados: {string.Join(", ", duplicates)}. " +
                "Cada pedido pode conter apenas um de cada item.");
        }
    }

    /// <summary>
    /// Busca os itens do cardápio pelos IDs, validando se todos existem.
    /// </summary>
    private async Task<List<MenuItem>> GetMenuItemsAsync(List<int> menuItemIds)
    {
        var menuItems = await _context.MenuItems
            .Where(m => menuItemIds.Contains(m.Id))
            .ToListAsync();

        var notFoundIds = menuItemIds.Except(menuItems.Select(m => m.Id)).ToList();
        if (notFoundIds.Count > 0)
        {
            throw new InvalidOrderException(
                $"Os seguintes IDs de itens não existem no cardápio: {string.Join(", ", notFoundIds)}.");
        }

        return menuItems;
    }

    /// <summary>
    /// Valida que o pedido respeita os limites: no máximo 1 sanduíche, 1 batata frita e 1 refrigerante.
    /// </summary>
    private static void ValidateItemQuantities(List<MenuItem> menuItems)
    {
        var sandwichCount = menuItems.Count(m => m.Type == MenuItemType.Sandwich);
        var sideCount = menuItems.Count(m => m.Type == MenuItemType.Side);
        var drinkCount = menuItems.Count(m => m.Type == MenuItemType.Drink);

        if (sandwichCount > 1)
            throw new InvalidOrderException("O pedido pode conter no máximo 1 sanduíche.");

        if (sideCount > 1)
            throw new InvalidOrderException("O pedido pode conter no máximo 1 acompanhamento (batata frita).");

        if (drinkCount > 1)
            throw new InvalidOrderException("O pedido pode conter no máximo 1 bebida (refrigerante).");
    }

    /// <summary>
    /// Calcula o percentual de desconto com base na composição do pedido.
    /// Regra 1 (20%): Sanduíche + Batata + Refrigerante.
    /// Regra 2 (15%): Sanduíche + Refrigerante.
    /// Regra 3 (10%): Sanduíche + Batata.
    /// Caso contrário: 0%.
    /// </summary>
    private static decimal CalculateDiscount(List<MenuItem> menuItems)
    {
        var hasSandwich = menuItems.Any(m => m.Type == MenuItemType.Sandwich);
        var hasSide = menuItems.Any(m => m.Type == MenuItemType.Side);
        var hasDrink = menuItems.Any(m => m.Type == MenuItemType.Drink);

        if (hasSandwich && hasSide && hasDrink)
            return 20m;

        if (hasSandwich && hasDrink)
            return 15m;

        if (hasSandwich && hasSide)
            return 10m;

        return 0m;
    }

    /// <summary>
    /// Mapeia uma entidade Order para o DTO OrderResponse.
    /// </summary>
    private static OrderResponse MapToResponse(Order order) => new()
    {
        Id = order.Id,
        Items = order.Items.Select(oi => new OrderItemResponse
        {
            MenuItemId = oi.MenuItem.Id,
            Name = oi.MenuItem.Name,
            Type = oi.MenuItem.Type.ToString(),
            Price = oi.MenuItem.Price
        }).ToList(),
        Subtotal = order.Subtotal,
        DiscountPercentage = order.DiscountPercentage,
        DiscountAmount = order.DiscountAmount,
        Total = order.Total,
        CreatedAt = order.CreatedAt,
        UpdatedAt = order.UpdatedAt
    };

    #endregion
}
