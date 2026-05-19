namespace BengosMenu.Models
{
    public class DishIngredient
    {
        public int DishId { get; set; }
        public int ProdusId { get; set; }
        
        public virtual Dish Dish { get; set; }
        public virtual Produs Produs { get; set; }
        
        public decimal Quantity { get; set; }
    }
}
