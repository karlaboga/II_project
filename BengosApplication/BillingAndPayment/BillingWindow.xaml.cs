using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Bengos.Models;
using Microsoft.Data.SqlClient;

namespace BillingAndPayment
{
    public partial class BillingWindow : Window
    {
        public ObservableCollection<OrderItem> OrderItems { get; } = new();
        public ObservableCollection<Dish> Dishes { get; } = new();
        private double _discountPercent;
        public double CurrentDiscount => _discountPercent;

        public BillingWindow(int? tableId = null, int? orderId = null, double discount = 0)
        {
            InitializeComponent();
            _discountPercent = discount;
            LoadDishes();
            CmbDish.ItemsSource = Dishes;
            DgOrder.ItemsSource = OrderItems;
        }

        private void LoadDishes()
        {
            Dishes.Clear();
            // SQL logic here
        }

        private void CmbDish_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
        private void BtnAddDish_Click(object sender, RoutedEventArgs e) { }
        private void DgOrder_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) { }
        private void BtnEditQty_Click(object sender, RoutedEventArgs e) { }
        private void BtnDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (DgOrder.SelectedItem is OrderItem item) OrderItems.Remove(item);
        }
        private void BtnDiscount_Click(object sender, RoutedEventArgs e) { }
        private void BtnPay_Click(object sender, RoutedEventArgs e) { }
        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();

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
}