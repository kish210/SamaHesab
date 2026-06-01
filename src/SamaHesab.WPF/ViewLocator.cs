using System.Windows;
using System.Windows.Controls;
using SamaHesab.WPF.ViewModels.Dashboard;
using SamaHesab.WPF.ViewModels.Accounting;
using SamaHesab.WPF.ViewModels.Inventory;
using SamaHesab.WPF.ViewModels.Sales;
using SamaHesab.WPF.ViewModels.Purchase;
using SamaHesab.WPF.ViewModels.POS;
using SamaHesab.WPF.ViewModels.CRM;
using SamaHesab.WPF.ViewModels.HRM;
using SamaHesab.WPF.ViewModels.Reports;
using SamaHesab.WPF.ViewModels.Settings;
using SamaHesab.WPF.Views.Dashboard;
using SamaHesab.WPF.Views.POS;

namespace SamaHesab.WPF;

public class ViewLocator : DataTemplateSelector
{
    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        if (item == null) return null;
        var type = item.GetType();

        var viewType = type.Name switch
        {
            nameof(DashboardViewModel)          => typeof(DashboardView),
            nameof(PosViewModel)                => typeof(PosView),
            _ => null
        };

        if (viewType == null) return CreateTextTemplate(type.Name);

        return new DataTemplate(type)
        {
            VisualTree = new FrameworkElementFactory(viewType)
        };
    }

    private static DataTemplate CreateTextTemplate(string name) =>
        new() { VisualTree = new FrameworkElementFactory(typeof(TextBlock)) };
}
