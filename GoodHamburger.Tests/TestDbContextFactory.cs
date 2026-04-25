using GoodHamburger.Api.Data;
using GoodHamburger.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Tests;

/// <summary>
/// Classe auxiliar para criar instâncias do AppDbContext para testes,
/// usando o provider InMemory do EF Core.
/// </summary>
public static class TestDbContextFactory
{
    /// <summary>
    /// Cria um AppDbContext com banco InMemory e seed do cardápio.
    /// </summary>
    public static AppDbContext Create(string? databaseName = null)
    {
        var dbName = databaseName ?? Guid.NewGuid().ToString();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();

        // Seed manual para InMemory (o HasData do OnModelCreating não é executado)
        if (!context.MenuItems.Any())
        {
            context.MenuItems.AddRange(
                new MenuItem { Id = 1, Name = "X Burger", Type = MenuItemType.Sandwich, Price = 5.00m },
                new MenuItem { Id = 2, Name = "X Egg", Type = MenuItemType.Sandwich, Price = 4.50m },
                new MenuItem { Id = 3, Name = "X Bacon", Type = MenuItemType.Sandwich, Price = 7.00m },
                new MenuItem { Id = 4, Name = "Batata Frita", Type = MenuItemType.Side, Price = 2.00m },
                new MenuItem { Id = 5, Name = "Refrigerante", Type = MenuItemType.Drink, Price = 2.50m }
            );
            context.SaveChanges();
        }

        return context;
    }
}
