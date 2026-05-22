using System.ComponentModel.DataAnnotations;

namespace BengosMenu.Models
{
    public class Dish
    {
        [Key]
        public int Id { get; set; }

        public required string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public required string Category { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public List<DishIngredient> DishIngredients { get; set; } = new List<DishIngredient>();
    }
}