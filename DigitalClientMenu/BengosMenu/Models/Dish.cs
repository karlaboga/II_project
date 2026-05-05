using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BengosMenu.Models
{
    [Table("Produs")]
    public class Produs
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = "pcs";
    }
    [Table("Dish")]
    public class Dish
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public ICollection<DishIngredient> DishIngredients { get; set; }
    }
    [Table("DishIngredient")]
    public class DishIngredient
    {
        public int DishId { get; set; }
        public int ProdusId { get; set; }
        public decimal Quantity { get; set; }
        public Dish Dish { get; set; }
        public Produs Produs { get; set; }
    }
}