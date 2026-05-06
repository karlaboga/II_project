using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace BengosRestaurantApp
{
    public partial class MenuWindow : Window
    {
        public ObservableCollection<MenuItem> AllMenuItems { get; set; }
        public ObservableCollection<MenuItem> FilteredItems { get; set; }

        public MenuWindow()
        {
            InitializeComponent();
            LoadMenuItems();
            FilteredItems = new ObservableCollection<MenuItem>(AllMenuItems);
            LvMenu.ItemsSource = FilteredItems;
            Loaded += MenuWindow_Loaded;
        }

        private async void MenuWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var publicIp = await IpHelper.GetPublicIpAddressAsync();
            TxtPublicIp.Text = $"Public IP: {publicIp}";
            ImgQrCode.Source = IpHelper.GenerateQrCode(publicIp);
        }

        private void LoadMenuItems()
        {
            AllMenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem { Name = "Margherita Pizza", Category = "Food", Price = 45.00,
                    Description = "Classic tomato sauce with mozzarella and fresh basil",
                    Ingredients = "Tomato sauce, Mozzarella, Basil, Olive oil" },
                new MenuItem { Name = "Caesar Salad", Category = "Food", Price = 32.00,
                    Description = "Fresh romaine lettuce with Caesar dressing and croutons",
                    Ingredients = "Romaine lettuce, Caesar dressing, Croutons, Parmesan" },
                new MenuItem { Name = "Grilled Salmon", Category = "Food", Price = 68.00,
                    Description = "Fresh salmon fillet grilled to perfection",
                    Ingredients = "Salmon, Lemon, Herbs, Butter" },
                new MenuItem { Name = "Cappuccino", Category = "Drinks", Price = 12.50,
                    Description = "Rich espresso with steamed milk and foam",
                    Ingredients = "Espresso, Steamed milk, Milk foam" },
                new MenuItem { Name = "Fresh Orange Juice", Category = "Drinks", Price = 15.00,
                    Description = "Freshly squeezed orange juice",
                    Ingredients = "Fresh oranges" },
                new MenuItem { Name = "Iced Tea", Category = "Drinks", Price = 10.00,
                    Description = "Refreshing iced tea with lemon",
                    Ingredients = "Black tea, Ice, Lemon, Mint" },
                new MenuItem { Name = "Tiramisu", Category = "Desserts", Price = 25.00,
                    Description = "Classic Italian dessert with coffee-soaked ladyfingers",
                    Ingredients = "Ladyfingers, Mascarpone, Coffee, Cocoa" },
                new MenuItem { Name = "Chocolate Cake", Category = "Desserts", Price = 22.00,
                    Description = "Rich chocolate cake with ganache",
                    Ingredients = "Chocolate, Flour, Eggs, Butter, Cream" },
                new MenuItem { Name = "Ice Cream", Category = "Desserts", Price = 18.00,
                    Description = "Vanilla ice cream with caramel sauce",
                    Ingredients = "Cream, Sugar, Vanilla, Caramel" }
            };
        }

        private void CategoryFilter_Changed(object sender, RoutedEventArgs e)
        {
            FilteredItems.Clear();

            string selectedCategory = "All";
            if (RbFood.IsChecked == true) selectedCategory = "Food";
            else if (RbDrinks.IsChecked == true) selectedCategory = "Drinks";
            else if (RbDesserts.IsChecked == true) selectedCategory = "Desserts";

            if (selectedCategory == "All")
            {
                foreach (var item in AllMenuItems)
                    FilteredItems.Add(item);
            }
            else
            {
                foreach (var item in AllMenuItems.Where(m => m.Category == selectedCategory))
                    FilteredItems.Add(item);
            }
        }

        private void BtnClose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class MenuItem
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string Ingredients { get; set; }
        public string PriceDisplay => $"${Price:0.00}";
    }
}
