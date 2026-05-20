namespace BillingAndPayment.Models;
public class Dish
{
    public string Name { get; set; } = "";
    public double Price { get; set; }
    public string DisplayText => $"{Name} — {Price:0.00} RON";
}