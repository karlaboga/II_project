namespace BillingAndPayment.Models;
public class Dish
{
    public string Name { get; set; } = "";
    public double Price { get; set; }
    public int Id { get; set; }
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
    public string PreparationTime { get; set; } = "--";
    public string Alergies { get; set; } = "None";
    public string Steps { get; set; } = "No steps provided.";
    public string DisplayText => $"{Name} — {Price:0.00} RON";
}