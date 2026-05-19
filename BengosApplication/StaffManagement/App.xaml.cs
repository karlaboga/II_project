using System.Windows;
namespace StaffManagement;
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        ShutdownMode = ShutdownMode.OnExplicitShutdown;
        base.OnStartup(e);
        var login = new LoginWindow();
        if (login.ShowDialog() == true)
        {
            var viewModel = new StaffViewModel(login.LoggedInUser, login.LoggedInRole);
            var main = new MainWindow(viewModel);
            main.Show();
        }
        else
        {
            Shutdown();
        }
    }
}