using CommunityToolkit.Mvvm.ComponentModel;
using SamaHesab.WPF.Services;
using SamaHesab.WPF.ViewModels.Shell;
namespace SamaHesab.WPF.ViewModels.HRM;
public partial class SalaryViewModel : BaseViewModel
{
    public SalaryViewModel(IDialogService d, INavigationService n) : base(d, n) { }
    public override Task LoadAsync() => Task.CompletedTask;
}
