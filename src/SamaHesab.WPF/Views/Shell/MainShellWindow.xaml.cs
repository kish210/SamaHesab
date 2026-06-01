using SamaHesab.WPF.ViewModels.Shell;
using System.Windows;
using System.Windows.Input;

namespace SamaHesab.WPF.Views.Shell;

public partial class MainShellWindow : Window
{
    public MainShellWindow(MainShellViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        // Keyboard shortcuts
        if (e.Key == Key.F2 && (Keyboard.Modifiers & ModifierKeys.None) == ModifierKeys.None)
        {
            // F2: New Sales Invoice
            if (DataContext is MainShellViewModel vm)
                vm.NavigateToCommand.Execute("NewSalesInvoice");
        }
        else if (e.Key == Key.F3)
        {
            // F3: POS
            if (DataContext is MainShellViewModel vm)
                vm.NavigateToCommand.Execute("POS");
        }
        else if (e.Key == Key.F12)
        {
            // F12: Dashboard
            if (DataContext is MainShellViewModel vm)
                vm.NavigateToCommand.Execute("Dashboard");
        }
    }
}
