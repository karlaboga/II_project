using Microsoft.EntityFrameworkCore;
using BengosMenu.Models;
namespace BengosMenu.Data;
public class RestaurantContext : DbContext
{
    public RestaurantContext(DbContextOptions<RestaurantContext> options) : base(options) { }
    public DbSet<Dish> Dishes => Set<Dish>();
    public DbSet<Produs> Produses => Set<Produs>();
    public DbSet<DishIngredient> DishIngredients => Set<DishIngredient>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DishIngredient>()
            .HasKey(di => new { di.DishId, di.ProdusId });
        modelBuilder.Entity<DishIngredient>()
            .HasOne(di => di.Dish).WithMany(d => d.DishIngredients)
            .HasForeignKey(di => di.DishId);
        modelBuilder.Entity<DishIngredient>()
            .HasOne(di => di.Produs).WithMany(p => p.DishIngredients)
            .HasForeignKey(di => di.ProdusId);
    }
}