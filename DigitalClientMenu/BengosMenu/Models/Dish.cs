using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Add this line
namespace BengosMenu.Models
{
    [Table("Produs")] // Maps to your dbo.Produs table
    public class Produs
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = "pcs";
    }
    [Table("Dish")] // Maps to your dbo.Dish table (fixes the "dbo.Dishes" error)
    public class Dish
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public ICollection<DishIngredient> DishIngredients { get; set; }
    }
    [Table("DishIngredient")] // Maps to your dbo.DishIngredient table
    public class DishIngredient
    {
        public int DishId { get; set; }
        public int ProdusId { get; set; }
        public decimal Quantity { get; set; }
        public Dish Dish { get; set; }
        public Produs Produs { get; set; }
    }
}