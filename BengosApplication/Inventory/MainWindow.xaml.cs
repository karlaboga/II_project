using System.Windows;
namespace Inventory;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new InventoryViewModel();
    }
    private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();

    private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {

    }
}