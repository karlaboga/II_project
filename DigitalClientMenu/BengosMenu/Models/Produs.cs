using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BengosMenu.Models
{
    public class Produs
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string Unit { get; set; }
        
        public virtual ICollection<DishIngredient> DishIngredients { get; set; }
    }
}
