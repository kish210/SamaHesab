using SamaHesab.WPF.ViewModels.Inventory;
using System.Windows.Controls;

namespace SamaHesab.WPF.Views.Inventory;

public partial class ProductListView : UserControl
{
    public ProductListView(ProductListViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (_, _) => await viewModel.LoadAsync();
    }
}
