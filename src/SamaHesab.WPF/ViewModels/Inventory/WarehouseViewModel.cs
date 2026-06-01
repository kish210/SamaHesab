using CommunityToolkit.Mvvm.ComponentModel;
using SamaHesab.WPF.Services;
using SamaHesab.WPF.ViewModels.Shell;
namespace SamaHesab.WPF.ViewModels.Inventory;
public partial class WarehouseViewModel : BaseViewModel
{
    public WarehouseViewModel(IDialogService d, INavigationService n) : base(d, n) { }
    public override Task LoadAsync() => Task.CompletedTask;
}
