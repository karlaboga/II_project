using System.Windows;
using System.Windows.Controls;
namespace BillingAndPayment;
public partial class PaymentWindow : Window
{
    private readonly double totalAmount;
    private double discountPercent;
    private bool isInitialized = false;
    public PaymentWindow(double total, double discount)
    {
        InitializeComponent();
        totalAmount = total;
        discountPercent = discount;
        TxtAmount.Text = $"${totalAmount:0.00}";
        isInitialized = true;
    }
    private void RbPayment_Checked(object sender, RoutedEventArgs e)
    {
        if (!isInitialized) return;
        CashPanel.Visibility = RbCash?.IsChecked == true
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
    private void TxtCashGiven_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (double.TryParse(TxtCashGiven.Text, out double given))
        {
            double changeAmount = given - totalAmount;
            TxtChange.Text = changeAmount >= 0 ? $"${changeAmount:0.00}" : "Not enough cash";
            TxtChange.Foreground = changeAmount >= 0
                ? System.Windows.Media.Brushes.DarkGreen
                : System.Windows.Media.Brushes.Red;
        }
        else
        {
            TxtChange.Text = "-";
            TxtChange.Foreground = System.Windows.Media.Brushes.Black;
        }
    }
    private void BtnConfirm_Click(object sender, RoutedEventArgs e)
    {
        if (RbCash.IsChecked == true)
        {
            if (!double.TryParse(TxtCashGiven.Text, out double given) || given < totalAmount)
            {
                MessageBox.Show("Please enter a valid cash amount that covers the total.",
                    "Invalid Amount", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            double changeAmount = given - totalAmount;
            MessageBox.Show(
                $"Payment confirmed!\n\nTotal: ${totalAmount:0.00}\n" +
                $"Cash Given: ${given:0.00}\nChange: ${changeAmount:0.00}",
                "Receipt", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show(
                $"Card payment of ${totalAmount:0.00} confirmed!\nThank you!",
                "Receipt", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        DialogResult = true;
        Close();
    }
    private void BtnBack_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}