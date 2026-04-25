using GoodHamburger.Api.Services;

namespace GoodHamburger.Tests;

/// <summary>
/// Testes unitários para o MenuService.
/// </summary>
public class MenuServiceTests
{
    [Fact]
    public async Task GetAll_ShouldReturnAllMenuItems()
    {
        // Arrange
        var context = TestDbContextFactory.Create();
        var service = new MenuService(context);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        Assert.Equal(5, result.Count());
    }

    [Fact]
    public async Task GetAll_ShouldContainAllExpectedItems()
    {
        // Arrange
        var context = TestDbContextFactory.Create();
        var service = new MenuService(context);

        // Act
        var items = (await service.GetAllAsync()).ToList();

        // Assert
        Assert.Contains(items, i => i.Name == "X Burger" && i.Price == 5.00m);
        Assert.Contains(items, i => i.Name == "X Egg" && i.Price == 4.50m);
        Assert.Contains(items, i => i.Name == "X Bacon" && i.Price == 7.00m);
        Assert.Contains(items, i => i.Name == "Batata Frita" && i.Price == 2.00m);
        Assert.Contains(items, i => i.Name == "Refrigerante" && i.Price == 2.50m);
    }

    [Fact]
    public async Task GetById_ExistingItem_ShouldReturnItem()
    {
        // Arrange
        var context = TestDbContextFactory.Create();
        var service = new MenuService(context);

        // Act
        var result = await service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("X Burger", result.Name);
        Assert.Equal(5.00m, result.Price);
        Assert.Equal("Sandwich", result.Type);
    }

    [Fact]
    public async Task GetById_NonExistingItem_ShouldReturnNull()
    {
        // Arrange
        var context = TestDbContextFactory.Create();
        var service = new MenuService(context);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }
}
