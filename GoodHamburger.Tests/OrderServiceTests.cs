using GoodHamburger.Api.DTOs;
using GoodHamburger.Api.Exceptions;
using GoodHamburger.Api.Services;

namespace GoodHamburger.Tests;

/// <summary>
/// Testes unitários para o OrderService, cobrindo:
/// - Cálculos de desconto (20%, 15%, 10%, 0%)
/// - Validações de negócio (duplicatas, limites de quantidade)
/// - Operações CRUD completas
/// </summary>
public class OrderServiceTests
{
    private OrderService CreateService(string? dbName = null)
    {
        var context = TestDbContextFactory.Create(dbName);
        return new OrderService(context);
    }

    #region Testes de Desconto

    [Fact]
    public async Task Create_SandwichSideDrink_ShouldApply20PercentDiscount()
    {
        // Arrange - X Burger (5.00) + Batata (2.00) + Refrigerante (2.50) = 9.50
        var service = CreateService();
        var request = new CreateOrderRequest { MenuItemIds = [1, 4, 5] };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.Equal(9.50m, result.Subtotal);
        Assert.Equal(20m, result.DiscountPercentage);
        Assert.Equal(1.90m, result.DiscountAmount);
        Assert.Equal(7.60m, result.Total);
    }

    [Fact]
    public async Task Create_SandwichAndDrink_ShouldApply15PercentDiscount()
    {
        // Arrange - X Egg (4.50) + Refrigerante (2.50) = 7.00
        var service = CreateService();
        var request = new CreateOrderRequest { MenuItemIds = [2, 5] };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.Equal(7.00m, result.Subtotal);
        Assert.Equal(15m, result.DiscountPercentage);
        Assert.Equal(1.05m, result.DiscountAmount);
        Assert.Equal(5.95m, result.Total);
    }

    [Fact]
    public async Task Create_SandwichAndSide_ShouldApply10PercentDiscount()
    {
        // Arrange - X Bacon (7.00) + Batata (2.00) = 9.00
        var service = CreateService();
        var request = new CreateOrderRequest { MenuItemIds = [3, 4] };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.Equal(9.00m, result.Subtotal);
        Assert.Equal(10m, result.DiscountPercentage);
        Assert.Equal(0.90m, result.DiscountAmount);
        Assert.Equal(8.10m, result.Total);
    }

    [Fact]
    public async Task Create_OnlySandwich_ShouldApplyNoDiscount()
    {
        // Arrange - Apenas X Burger (5.00)
        var service = CreateService();
        var request = new CreateOrderRequest { MenuItemIds = [1] };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.Equal(5.00m, result.Subtotal);
        Assert.Equal(0m, result.DiscountPercentage);
        Assert.Equal(0m, result.DiscountAmount);
        Assert.Equal(5.00m, result.Total);
    }

    [Fact]
    public async Task Create_OnlySideAndDrink_ShouldApplyNoDiscount()
    {
        // Arrange - Batata (2.00) + Refrigerante (2.50) = 4.50
        var service = CreateService();
        var request = new CreateOrderRequest { MenuItemIds = [4, 5] };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.Equal(4.50m, result.Subtotal);
        Assert.Equal(0m, result.DiscountPercentage);
        Assert.Equal(0m, result.DiscountAmount);
        Assert.Equal(4.50m, result.Total);
    }

    [Fact]
    public async Task Create_OnlyDrink_ShouldApplyNoDiscount()
    {
        // Arrange - Apenas Refrigerante (2.50)
        var service = CreateService();
        var request = new CreateOrderRequest { MenuItemIds = [5] };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.Equal(2.50m, result.Subtotal);
        Assert.Equal(0m, result.DiscountPercentage);
        Assert.Equal(0m, result.DiscountAmount);
        Assert.Equal(2.50m, result.Total);
    }

    #endregion

    #region Testes de Validação

    [Fact]
    public async Task Create_DuplicateItems_ShouldThrowDuplicateItemException()
    {
        // Arrange - Dois X Burgers
        var service = CreateService();
        var request = new CreateOrderRequest { MenuItemIds = [1, 1] };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DuplicateItemException>(
            () => service.CreateAsync(request));
        Assert.Contains("duplicados", ex.Message);
    }

    [Fact]
    public async Task Create_TwoSandwiches_ShouldThrowInvalidOrderException()
    {
        // Arrange - X Burger + X Egg (dois sanduíches diferentes)
        var service = CreateService();
        var request = new CreateOrderRequest { MenuItemIds = [1, 2] };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOrderException>(
            () => service.CreateAsync(request));
        Assert.Contains("sanduíche", ex.Message);
    }

    [Fact]
    public async Task Create_InvalidMenuItemId_ShouldThrowInvalidOrderException()
    {
        // Arrange - ID 999 não existe no cardápio
        var service = CreateService();
        var request = new CreateOrderRequest { MenuItemIds = [999] };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOrderException>(
            () => service.CreateAsync(request));
        Assert.Contains("não existem", ex.Message);
    }

    #endregion

    #region Testes de CRUD

    [Fact]
    public async Task GetAll_ShouldReturnAllOrders()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        var service = CreateService(dbName);

        await service.CreateAsync(new CreateOrderRequest { MenuItemIds = [1] });
        await service.CreateAsync(new CreateOrderRequest { MenuItemIds = [2, 4] });

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetById_ExistingOrder_ShouldReturnOrder()
    {
        // Arrange
        var service = CreateService();
        var created = await service.CreateAsync(new CreateOrderRequest { MenuItemIds = [1, 4, 5] });

        // Act
        var result = await service.GetByIdAsync(created.Id);

        // Assert
        Assert.Equal(created.Id, result.Id);
        Assert.Equal(3, result.Items.Count);
    }

    [Fact]
    public async Task GetById_NonExistingOrder_ShouldThrowOrderNotFoundException()
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<OrderNotFoundException>(
            () => service.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task Update_ShouldRecalculateDiscount()
    {
        // Arrange - Criar com sanduíche apenas (0% desc.)
        var service = CreateService();
        var created = await service.CreateAsync(new CreateOrderRequest { MenuItemIds = [1] });
        Assert.Equal(0m, created.DiscountPercentage);

        // Act - Atualizar para sanduíche + batata + refri (20% desc.)
        var updated = await service.UpdateAsync(created.Id,
            new UpdateOrderRequest { MenuItemIds = [1, 4, 5] });

        // Assert
        Assert.Equal(20m, updated.DiscountPercentage);
        Assert.Equal(9.50m, updated.Subtotal);
        Assert.Equal(7.60m, updated.Total);
        Assert.NotNull(updated.UpdatedAt);
    }

    [Fact]
    public async Task Delete_ExistingOrder_ShouldRemoveOrder()
    {
        // Arrange
        var service = CreateService();
        var created = await service.CreateAsync(new CreateOrderRequest { MenuItemIds = [1] });

        // Act
        await service.DeleteAsync(created.Id);

        // Assert
        await Assert.ThrowsAsync<OrderNotFoundException>(
            () => service.GetByIdAsync(created.Id));
    }

    [Fact]
    public async Task Delete_NonExistingOrder_ShouldThrowOrderNotFoundException()
    {
        // Arrange
        var service = CreateService();

        // Act & Assert
        await Assert.ThrowsAsync<OrderNotFoundException>(
            () => service.DeleteAsync(Guid.NewGuid()));
    }

    #endregion
}
