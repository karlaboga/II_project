using System.ComponentModel.DataAnnotations;
namespace BengosMenu.Models;
public class Produs
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Unit { get; set; } = "";
    public string? Category { get; set; }

    public int Quantity { get; set; }
    public int MinStock { get; set; }
    public List<DishIngredient> DishIngredients { get; set; } = new();
}
