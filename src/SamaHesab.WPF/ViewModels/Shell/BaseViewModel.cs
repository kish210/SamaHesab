using CommunityToolkit.Mvvm.ComponentModel;
using SamaHesab.WPF.Services;

namespace SamaHesab.WPF.ViewModels.Shell;

public abstract partial class BaseViewModel : ObservableObject
{
    protected readonly IDialogService _dialogService;
    protected readonly INavigationService _navigationService;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string _loadingMessage = "در حال بارگذاری...";

    protected BaseViewModel(IDialogService dialogService, INavigationService navigationService)
    { _dialogService = dialogService; _navigationService = navigationService; }

    public virtual Task LoadAsync() => Task.CompletedTask;

    protected async Task ExecuteAsync(Func<Task> action, string loadingMsg = "در حال پردازش...")
    {
        IsLoading = true; LoadingMessage = loadingMsg;
        try { await action(); }
        finally { IsLoading = false; }
    }
}
