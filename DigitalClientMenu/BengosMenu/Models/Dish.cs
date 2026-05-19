using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BengosMenu.Models
{
    public class Dish
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public decimal Price { get; set; }
        
        [Required]
        public string Category { get; set; }
        
        public string ImageUrl { get; set; }
        
        public virtual ICollection<DishIngredient> DishIngredients { get; set; }
    }
}
