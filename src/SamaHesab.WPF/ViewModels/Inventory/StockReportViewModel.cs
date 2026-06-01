using CommunityToolkit.Mvvm.ComponentModel;
using SamaHesab.WPF.Services;
using SamaHesab.WPF.ViewModels.Shell;
namespace SamaHesab.WPF.ViewModels.Inventory;
public partial class StockReportViewModel : BaseViewModel
{
    public StockReportViewModel(IDialogService d, INavigationService n) : base(d, n) { }
    public override Task LoadAsync() => Task.CompletedTask;
}
