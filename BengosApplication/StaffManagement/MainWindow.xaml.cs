using System.Windows;
namespace StaffManagement;
public partial class MainWindow : Window
{
    public MainWindow(StaffViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}