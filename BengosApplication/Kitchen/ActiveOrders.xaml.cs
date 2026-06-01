using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Kitchen.Models1;

namespace Kitchen
{
    /// <summary>
    /// Interaction logic for ActiveOrders.xaml
    /// </summary>
    public partial class ActiveOrders : Window
    {
        private readonly string connString = @"Server=tcp:server-proiect-bengos-ii.database.windows.net,1433;Initial Catalog=BengosDB;User ID=admin-proiect;Password=Bengos67;Encrypt=True;TrustServerCertificate=False;";

        public ActiveOrders()
        {
            InitializeComponent();
            LoadActiveOrders();
        }

        private void LoadActiveOrders()
        {
            List<KitchenOrderViewModel> activeOrders = new();

            try
            {
                using var conn = new SqlConnection(connString);
                conn.Open();

                // 1. Selectăm Id-ul comenzii, data și numărul mesei prin INNER JOIN
                // Ne asigurăm că selectăm exact o.Id, o.OrderDate și t.TableNumber
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
                Dictionary<int, int> tableNumbers = new(); // Dicționar pentru a reține masa fiecărei comenzi

                while (rdrOrders.Read())
                {
                    // Citim coloanele exact după numele date în clauza SELECT de mai sus
                    int id = Convert.ToInt32(rdrOrders["OrderId"]);
                    orderIds.Add(id);
                    orderTimes[id] = Convert.ToDateTime(rdrOrders["Timestamp"]);
                    tableNumbers[id] = Convert.ToInt32(rdrOrders["TableNumber"]); // Acum va fi găsită garantat!
                }
                rdrOrders.Close(); // Închidem obligatoriu cititorul înainte de următorul pas

                // 2. Pentru fiecare comandă găsită, încărcăm produsele sale
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

                        // Formatăm numele preparatului cu tot cu alergeni pentru interfața ta originală
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

                    // Calculăm timpul mediu de pregătire
                    double averageTime = prepTimes.Count > 0 ? prepTimes.Average() : 0;

                    // Împachetăm totul în ViewModel-ul final
                    activeOrders.Add(new KitchenOrderViewModel
                    {
                        OrderId = $"Order #{oid}",
                        TableNumber = tableNumbers[oid], // Transmitem numărul mesei salvat anterior
                        Timestamp = orderTimes[oid],
                        TotalPrepTime = (int)Math.Round(averageTime),
                        Items = orderItems
                    });
                }
            }
            catch (Exception ex)
            {
                // Afișează întreaga eroare detaliată (inclusiv unde a crăpat) în caz că mai apare ceva
                MessageBox.Show("Eroare la încărcarea datelor: " + ex.Message, "Eroare SQL", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Trimitem lista curată către interfața grafică (UI Mov)
            OrdersControl.ItemsSource = null;
            OrdersControl.ItemsSource = activeOrders;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnCompleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is KitchenOrderViewModel completedOrder)
            {
                string idString = completedOrder.OrderId.Replace("Order #", "").Trim();
                if (int.TryParse(idString, out int oid))
                {
                    try
                    {
                        using var conn = new SqlConnection(connString);
                        conn.Open();

                        using var cmd = new SqlCommand("UPDATE Orders SET Status = 'ReadyToServe' WHERE Id = @oid", conn);
                        cmd.Parameters.AddWithValue("@oid", oid);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show($"Comanda #{oid} a fost finalizată!");
                        LoadActiveOrders();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Eroare: " + ex.Message);
                    }
                }
            }
        }
    }

    
}