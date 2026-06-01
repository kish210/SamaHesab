using CommunityToolkit.Mvvm.ComponentModel;
using SamaHesab.WPF.Services;
using SamaHesab.WPF.ViewModels.Shell;
namespace SamaHesab.WPF.ViewModels.Accounting;
public partial class BankAccountViewModel : BaseViewModel
{
    public BankAccountViewModel(IDialogService dialogService, INavigationService navigationService)
        : base(dialogService, navigationService) { }
    public override Task LoadAsync() => Task.CompletedTask;
}
