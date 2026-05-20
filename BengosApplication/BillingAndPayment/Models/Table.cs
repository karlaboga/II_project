namespace BillingAndPayment.Models;
public class Table
{
    public int Id { get; set; }
    public int TableNumber { get; set; }
    public int Capacity { get; set; }
    public string Status { get; set; } = "Free";
    public string DisplayInfo => $"Table {TableNumber} — {Capacity} seats";
    public string StatusDisplay => Status == "Free" ? "✅ Free" : "🔴 Occupied";
    public string ActionText => Status == "Free" ? "New Order" : "View Bill";
    public string ActionColor => Status == "Free" ? "#D4A574" : "#8B5E3C";
}