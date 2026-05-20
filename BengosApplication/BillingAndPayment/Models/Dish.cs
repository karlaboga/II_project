namespace BillingAndPayment.Models;
public class Dish
{
    public string Name { get; set; } = "";
    public double Price { get; set; }
    public string DisplayText => $"{Name} — ${Price:0.00}";
    public int Id { get; set; }
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
    public string PreparationTime { get; set; } = "--";
    public string Alergies { get; set; } = "None";
}