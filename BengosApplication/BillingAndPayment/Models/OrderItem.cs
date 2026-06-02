namespace BillingAndPayment.Models
{
    public class OrderItem
    {
        public string Name { get; set; } = "";
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string PriceDisplay => $"{Price:0.00} RON";
        public string TotalDisplay => $"{Quantity * Price:0.00} RON";
        public double Total => Quantity * Price;

        public string StatusItem { get; set; } = "Pending";
    }
}
