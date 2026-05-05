using BengosMenu.Models;
using System.Data.Entity;

namespace BengostMenu.Data
{
    public class RestaurantContext : DbContext
    {
        // Matches connection string name in web.config
        public RestaurantContext() : base("name=RestaurantDBEntities") { }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Produs> Produse { get; set; }
        public DbSet<DishIngredient> DishIngredients { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DishIngredient>()
                .HasKey(di => new { di.DishId, di.ProdusId });
            modelBuilder.Entity<DishIngredient>()
                .HasRequired(di => di.Dish)
                .WithMany(d => d.DishIngredients)
                .HasForeignKey(di => di.DishId);
            modelBuilder.Entity<DishIngredient>()
                .HasRequired(di => di.Produs)
                .WithMany()
                .HasForeignKey(di => di.ProdusId);
        }
    }
}