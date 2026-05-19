using System.Windows;
namespace Inventory;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new InventoryViewModel();
    }
}