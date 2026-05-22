using System.ComponentModel.DataAnnotations;

namespace BengosMenu.Models
{
    public class Produs
    {
        [Key]
        public int Id { get; set; }

        public required string Name { get; set; } = string.Empty;

        public required string Unit { get; set; } = string.Empty;

        public string? Category { get; set; }

        public int Quantity { get; set; }

        public int MinStock { get; set; }

        public List<DishIngredient> DishIngredients { get; set; } = new List<DishIngredient>();
    }
}