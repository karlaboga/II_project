using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace BillingAndPayment;

public partial class PaymentWindow : Window
{
    private readonly string connString = @"Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;";

    private readonly double totalAmount;
    private double discountPercent;
    private readonly int tableId;  // Salvăm ID-ul mesei curente
    private readonly int orderId;  // Salvăm ID-ul comenzii curente
    private bool isInitialized = false;

    // Actualizăm constructorul pentru a primi tableId și orderId de la BillingWindow / TableWindow
    public PaymentWindow(double total, double discount, int tableId, int orderId)
    {
        InitializeComponent();
        totalAmount = total;
        discountPercent = discount;
        this.tableId = tableId;
        this.orderId = orderId;

        TxtAmount.Text = $"{totalAmount:0.00} RON";
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
            TxtChange.Text = changeAmount >= 0 ? $"{changeAmount:0.00} RON" : "Not enough cash";
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
                $"Payment confirmed!\n\nTotal: {totalAmount:0.00} RON\n" +
                $"Cash Given: {given:0.00} RON\nChange: {changeAmount:0.00} RON",
                "Receipt", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show(
                $"Card payment of {totalAmount:0.00} RON confirmed!\nThank you!",
                "Receipt", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ========================================================
        // ACTUAlIZARE BAZĂ DE DATE: Setăm comanda ca plătită și masa ca liberă
        // ========================================================
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();

            // 1. Schimbăm statusul comenzii curente în 'Paid' și actualizăm totalul final
            string updateOrderQuery = "UPDATE Orders SET Total = @total, Status = 'Paid' WHERE Id = @oid";
            using var cmdOrder = new SqlCommand(updateOrderQuery, conn);
            cmdOrder.Parameters.AddWithValue("@total", totalAmount);
            cmdOrder.Parameters.AddWithValue("@oid", orderId);
            cmdOrder.ExecuteNonQuery();

            // 2. Schimbăm statusul mesei în 'Free'
            string updateTableQuery = "UPDATE Tables SET Status = 'Free' WHERE Id = @tid";
            using var cmdTable = new SqlCommand(updateTableQuery, conn);
            cmdTable.Parameters.AddWithValue("@tid", tableId);
            cmdTable.ExecuteNonQuery();

            // 3. Forțăm interfața principală cu hărțile de mese (TableWindow) să redeseneze imediat mesele
            if (TableWindow.Instance != null)
            {
                TableWindow.Instance.LoadTables();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Eroare la finalizarea plății în baza de date: " + ex.Message, "Eroare SQL", MessageBoxButton.OK, MessageBoxImage.Error);
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