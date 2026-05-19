using System.ComponentModel.DataAnnotations;
namespace BengosMenu.Models
{
    public class DishIngredient
    {
        public int DishId { get; set; }
        public int ProdusId { get; set; }
        public decimal Quantity { get; set; }
        public Dish Dish { get; set; }
        public Produs Produs { get; set; }
    }
}
