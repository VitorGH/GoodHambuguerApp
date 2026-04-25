using GoodHamburger.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Api.Data;

/// <summary>
/// Contexto do Entity Framework Core para acesso ao banco de dados.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<MenuItem> MenuItems { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da entidade MenuItem
        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Name).IsRequired().HasMaxLength(100);
            entity.Property(m => m.Price).HasColumnType("decimal(10,2)");
            entity.Property(m => m.Type).HasConversion<string>().HasMaxLength(20);
        });

        // Configuração da entidade Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Subtotal).HasColumnType("decimal(10,2)");
            entity.Property(o => o.DiscountPercentage).HasColumnType("decimal(5,2)");
            entity.Property(o => o.DiscountAmount).HasColumnType("decimal(10,2)");
            entity.Property(o => o.Total).HasColumnType("decimal(10,2)");
            entity.HasMany(o => o.Items)
                  .WithOne(oi => oi.Order)
                  .HasForeignKey(oi => oi.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração da entidade OrderItem
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi => oi.Id);
            entity.HasOne(oi => oi.MenuItem)
                  .WithMany()
                  .HasForeignKey(oi => oi.MenuItemId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed dos itens do cardápio
        modelBuilder.Entity<MenuItem>().HasData(
            new MenuItem { Id = 1, Name = "X Burger", Type = MenuItemType.Sandwich, Price = 5.00m },
            new MenuItem { Id = 2, Name = "X Egg", Type = MenuItemType.Sandwich, Price = 4.50m },
            new MenuItem { Id = 3, Name = "X Bacon", Type = MenuItemType.Sandwich, Price = 7.00m },
            new MenuItem { Id = 4, Name = "Batata Frita", Type = MenuItemType.Side, Price = 2.00m },
            new MenuItem { Id = 5, Name = "Refrigerante", Type = MenuItemType.Drink, Price = 2.50m }
        );
    }
}
