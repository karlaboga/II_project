using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using BillingAndPayment.Models;
namespace BillingAndPayment;
public partial class TableWindow : Window
{
    private readonly string connString = @"Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;";
    public TableWindow()
    {
        InitializeComponent();
        LoadTables();
    }
    private void LoadTables()
    {
        var tables = new List<Table>();
        try
        {
            using var conn = new SqlConnection(connString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT Id, TableNumber, Capacity, Status FROM Tables ORDER BY TableNumber", conn);
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                tables.Add(new Table
                {
                    Id = Convert.ToInt32(rdr["Id"]),
                    TableNumber = Convert.ToInt32(rdr["TableNumber"]),
                    Capacity = Convert.ToInt32(rdr["Capacity"]),
                    Status = rdr["Status"]?.ToString() ?? "Free"
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error loading tables: " + ex.Message);
        }
        TableList.ItemsSource = tables;
    }
    private void BtnTableAction_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is int tableId)
        {
            var billing = new BillingWindow(tableId) { Owner = this };
            billing.ShowDialog();
            LoadTables();
        }
    }
    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
}