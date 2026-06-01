namespace BillingAndPayment.Models
{
    public class OrderItem
    {
        public string Name { get; set; } = "";
        public int Quantity { get; set; } = 1;
        public double Price { get; set; }
        public double Total => Quantity * Price;
        public string PriceDisplay => $"{Price:0.00} RON";
        public string TotalDisplay => $"{Total:0.00} RON";
    }
}
