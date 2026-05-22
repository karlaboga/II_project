using System.ComponentModel.DataAnnotations;
using Bengos.Models;

namespace Bengos.Models
{
    public class DishIngredient
    {
        public int DishId { get; set; }
        public int ProdusId { get; set; }
        public decimal Quantity { get; set; }
        public virtual Dish Dish { get; set; } = null!;
        public virtual Produs Produs { get; set; } = null!;
    }
}
