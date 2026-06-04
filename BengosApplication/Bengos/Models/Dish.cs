namespace Bengos.Models;

public class Dish
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public double Price { get; set; }
    public string PreparationTime { get; set; } = "";
    public string Alergies { get; set; } = "";
    public string Steps { get; set; } = "";
}
