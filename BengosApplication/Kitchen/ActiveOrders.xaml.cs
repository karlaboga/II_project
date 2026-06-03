using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Kitchen.Models1;

namespace Kitchen
{
    public partial class ActiveOrders : Window
    {
        private readonly string connString = @"Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;";

        private DispatcherTimer autoRefreshTimer;

        public ActiveOrders()
        {
            InitializeComponent();
            LoadActiveOrders();
            SetupAutoRefresh();
        }

        private void SetupAutoRefresh()
        {
            autoRefreshTimer = new DispatcherTimer();
            autoRefreshTimer.Interval = TimeSpan.FromSeconds(10);
            autoRefreshTimer.Tick += AutoRefreshTimer_Tick;
            autoRefreshTimer.Start();
        }

        private void AutoRefreshTimer_Tick(object sender, EventArgs e)
        {
            LoadActiveOrders();
        }

        private void LoadActiveOrders()
        {
            List<KitchenOrderViewModel> activeOrders = new();

            try
            {
                using var conn = new SqlConnection(connString);
                conn.Open();

                string orderQuery = @"
                    SELECT 
                        o.Id AS OrderId, 
                        o.OrderNumber,
                        ISNULL(o.OrderDate, GETDATE()) AS Timestamp, 
                        t.TableNumber AS TableNumber
                    FROM Orders o
                    INNER JOIN Tables t ON o.TableId = t.Id
                    WHERE o.Status = 'ToKitchen' 
                    ORDER BY o.OrderDate ASC";

                using var cmdOrders = new SqlCommand(orderQuery, conn);
                using var rdrOrders = cmdOrders.ExecuteReader();

                List<int> orderIds = new();
                Dictionary<int, DateTime> orderTimes = new();
                Dictionary<int, int> tableNumbers = new();
                Dictionary<int, int> orderNumbers = new();

                while (rdrOrders.Read())
                {
                    int id = Convert.ToInt32(rdrOrders["OrderId"]);
                    orderIds.Add(id);
                    orderTimes[id] = Convert.ToDateTime(rdrOrders["Timestamp"]);
                    tableNumbers[id] = Convert.ToInt32(rdrOrders["TableNumber"]);
                    orderNumbers[id] = Convert.ToInt32(rdrOrders["OrderNumber"]);
                }
                rdrOrders.Close();

                foreach (int oid in orderIds)
                {
                    var orderItems = new List<KitchenItem>();
                    List<int> prepTimes = new();

                    string itemsQuery = @"
                        SELECT oi.Name, oi.Quantity, 
                               ISNULL(d.PreparationTime, 0) as PrepTime, 
                               d.Alergies
                        FROM OrderItems oi
                        LEFT JOIN Dishes d ON oi.Name = d.Name
                        WHERE oi.OrderId = @oid AND oi.StatusItem = 'Pending'";

                    using var cmdItems = new SqlCommand(itemsQuery, conn);
                    cmdItems.Parameters.AddWithValue("@oid", oid);
                    using var rdrItems = cmdItems.ExecuteReader();

                    while (rdrItems.Read())
                    {
                        string baseName = rdrItems["Name"].ToString();
                        int qty = Convert.ToInt32(rdrItems["Quantity"]);
                        int pTime = Convert.ToInt32(rdrItems["PrepTime"]);
                        string allergies = rdrItems["Alergies"]?.ToString();

                        if (pTime > 0) prepTimes.Add(pTime);

                        string displayName = !string.IsNullOrEmpty(allergies) && allergies != "Nespecificat"
                            ? $"{baseName} (Alergeni: {allergies})"
                            : baseName;

                        orderItems.Add(new KitchenItem
                        {
                            Quantity = qty,
                            Name = displayName
                        });
                    }
                    rdrItems.Close();

                    double averageTime = prepTimes.Count > 0 ? prepTimes.Average() : 0;

                    activeOrders.Add(new KitchenOrderViewModel
                    {
                        OrderDisplay = $"Order #{orderNumbers[oid]}",
                        OrderNumber = orderNumbers[oid],
                        TableNumber = tableNumbers[oid],
                        Timestamp = orderTimes[oid],
                        TotalPrepTime = (int)Math.Round(averageTime),
                        Items = orderItems
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Eroare fundal SQL: " + ex.Message);
            }

            OrdersControl.ItemsSource = null;
            OrdersControl.ItemsSource = activeOrders;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            autoRefreshTimer?.Stop();
            this.Close();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadActiveOrders();
        }

        private void BtnCompleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is KitchenOrderViewModel completedOrder)
            {
                try
                {
                    using var conn = new SqlConnection(connString);
                    conn.Open();

                    using var getIdCmd = new SqlCommand(
                        "SELECT Id FROM Orders WHERE OrderNumber=@onum AND Status='ToKitchen'", conn);
                    getIdCmd.Parameters.AddWithValue("@onum", completedOrder.OrderNumber);
                    var result = getIdCmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("Comanda nu a fost găsită în baza de date.");
                        return;
                    }
                    int oid = Convert.ToInt32(result);

                    var insufficient = new List<string>();
                    using (var checkCmd = new SqlCommand(@"
                        SELECT d.Name AS DishName, p.Name AS Ingredient,
                               SUM(di.QuantityRequired * oi.Quantity) AS Needed,
                               MAX(p.Quantity) AS Available
                        FROM OrderItems oi
                        INNER JOIN Dishes d ON oi.Name = d.Name
                        INNER JOIN DishIngredients di ON di.DishId = d.Id
                        INNER JOIN Produses p ON p.Id = di.ProductId
                        WHERE oi.OrderId = @oid AND oi.StatusItem = 'Pending'
                        GROUP BY d.Name, p.Name
                        HAVING MAX(p.Quantity) < SUM(di.QuantityRequired * oi.Quantity)", conn))
                    {
                        checkCmd.Parameters.AddWithValue("@oid", oid);
                        using var checkRdr = checkCmd.ExecuteReader();
                        while (checkRdr.Read())
                        {
                            string dish = checkRdr["DishName"]?.ToString() ?? "";
                            string ing = checkRdr["Ingredient"]?.ToString() ?? "";
                            double needed = Convert.ToDouble(checkRdr["Needed"]);
                            double available = Convert.ToDouble(checkRdr["Available"]);
                            insufficient.Add($"  • {dish}: needs {needed}× {ing}, only {available} in stock");
                        }
                    }

                    if (insufficient.Count > 0)
                    {
                        MessageBox.Show(
                            "Cannot complete order — insufficient stock:\n" +
                            string.Join("\n", insufficient),
                            "Insufficient Stock", MessageBoxButton.OK, MessageBoxImage.Warning);
                        LoadActiveOrders();
                        return;
                    }

                    using var transaction = conn.BeginTransaction();
                    try
                    {
                        using var cmd = new SqlCommand(
                            "UPDATE Orders SET Status = 'ReadyToServe' WHERE Id = @oid", conn, transaction);
                        cmd.Parameters.AddWithValue("@oid", oid);
                        cmd.ExecuteNonQuery();

                        using var cmdItems = new SqlCommand(
                            "UPDATE OrderItems SET StatusItem = 'Ready' WHERE OrderId = @oid AND StatusItem = 'Pending'", conn, transaction);
                        cmdItems.Parameters.AddWithValue("@oid", oid);
                        cmdItems.ExecuteNonQuery();

                        string deductStock = @"
                            UPDATE p
                            SET p.Quantity = p.Quantity - (di.QuantityRequired * oi.Quantity)
                            FROM dbo.Produses p
                            INNER JOIN dbo.DishIngredients di ON p.Id = di.ProductId
                            INNER JOIN dbo.Dishes d ON di.DishId = d.Id
                            INNER JOIN dbo.OrderItems oi ON d.Name = oi.Name
                            WHERE oi.OrderId = @oid AND oi.StatusItem = 'Pending'";

                        using var cmdDeduct = new SqlCommand(deductStock, conn, transaction);
                        cmdDeduct.Parameters.AddWithValue("@oid", oid);
                        cmdDeduct.ExecuteNonQuery();

                        transaction.Commit();

                        MessageBox.Show($"Produsele din comanda #{completedOrder.OrderNumber} sunt gata, iar ingredientele au fost scăzute din stoc!",
                                        "Comandă Finalizată", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadActiveOrders();

                        if (BillingAndPayment.TableWindow.Instance != null)
                            BillingAndPayment.TableWindow.Instance.LoadTables();
                    }
                    catch
                    {
                        transaction.Rollback();
                        MessageBox.Show("Eroare la finalizarea comenzii și scăderea stocului.",
                                        "Eroare", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Eroare la conexiune: " + ex.Message);
                }
            }
        }
    }
}