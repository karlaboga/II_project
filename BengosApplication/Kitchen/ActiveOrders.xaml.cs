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

                while (rdrOrders.Read())
                {
                    int id = Convert.ToInt32(rdrOrders["OrderId"]);
                    orderIds.Add(id);
                    orderTimes[id] = Convert.ToDateTime(rdrOrders["Timestamp"]);
                    tableNumbers[id] = Convert.ToInt32(rdrOrders["TableNumber"]);
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
                        WHERE oi.OrderId = @oid";

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
                        OrderId = $"Order #{oid}",
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
                string idString = completedOrder.OrderId.Replace("Order #", "").Trim();
                if (int.TryParse(idString, out int oid))
                {
                    using var conn = new SqlConnection(connString);
                    conn.Open();
                    using var transaction = conn.BeginTransaction();

                    try
                    {
                        // 1. Schimbăm statusul comenzii principale în 'ReadyToServe'
                        using var cmd = new SqlCommand("UPDATE Orders SET Status = 'ReadyToServe' WHERE Id = @oid", conn, transaction);
                        cmd.Parameters.AddWithValue("@oid", oid);
                        cmd.ExecuteNonQuery();

                        // 2. Schimbăm statusul produselor din comandă în 'Ready'
                        string updateItemsQuery = "UPDATE OrderItems SET StatusItem = 'Ready' WHERE OrderId = @oid";
                        using var cmdItems = new SqlCommand(updateItemsQuery, conn, transaction);
                        cmdItems.Parameters.AddWithValue("@oid", oid);
                        cmdItems.ExecuteNonQuery();

                        // 3. --- SCĂDEREA AUTOMATĂ A INVENTARULUI (CORECTATĂ) ---
                        // Legăm corect: ItemComandat -> NumeReteta -> IngredienteReteta -> ProdusDinInventar
                        string deductStockQuery = @"
                            UPDATE p
                            SET p.Quantity = p.Quantity - (di.QuantityRequired * oi.Quantity)
                            FROM dbo.Produses p
                            INNER JOIN dbo.DishIngredients di ON p.Id = di.ProductId
                            INNER JOIN dbo.Dishes d ON di.DishId = d.Id
                            INNER JOIN dbo.OrderItems oi ON d.Name = oi.Name
                            WHERE oi.OrderId = @oid";

                        using var cmdDeduct = new SqlCommand(deductStockQuery, conn, transaction);
                        cmdDeduct.Parameters.AddWithValue("@oid", oid);
                        cmdDeduct.ExecuteNonQuery();
                        // ------------------------------------------------------

                        transaction.Commit();

                        MessageBox.Show($"Produsele din comanda #{oid} sunt gata, iar ingredientele au fost scăzute din stoc!", "Comandă Finalizată", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadActiveOrders();

                        if (BillingAndPayment.TableWindow.Instance != null)
                        {
                            BillingAndPayment.TableWindow.Instance.LoadTables();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Eroare la finalizarea comenzii și scăderea stocului: " + ex.Message, "Eroare Core", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}