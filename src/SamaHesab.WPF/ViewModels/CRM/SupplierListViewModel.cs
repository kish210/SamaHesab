using CommunityToolkit.Mvvm.ComponentModel;
using SamaHesab.WPF.Services;
using SamaHesab.WPF.ViewModels.Shell;
namespace SamaHesab.WPF.ViewModels.CRM;
public partial class SupplierListViewModel : BaseViewModel
{
    public SupplierListViewModel(IDialogService d, INavigationService n) : base(d, n) { }
    public override Task LoadAsync() => Task.CompletedTask;
}
