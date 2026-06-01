using System.Windows.Controls;

namespace SamaHesab.WPF.Services;

public interface INavigationService
{
    event EventHandler<NavigationEventArgs>? Navigated;
    void NavigateTo(string viewName, object? parameter = null);
    void NavigateBack();
    bool CanNavigateBack { get; }
    string CurrentView { get; }
}

public class NavigationEventArgs : EventArgs
{
    public string ViewName { get; }
    public object? Parameter { get; }

    public NavigationEventArgs(string viewName, object? parameter = null)
    {
        ViewName = viewName;
        Parameter = parameter;
    }
}

public class NavigationService : INavigationService
{
    private readonly Stack<string> _navigationStack = new();
    public event EventHandler<NavigationEventArgs>? Navigated;

    public string CurrentView { get; private set; } = "Dashboard";
    public bool CanNavigateBack => _navigationStack.Count > 1;

    public void NavigateTo(string viewName, object? parameter = null)
    {
        _navigationStack.Push(viewName);
        CurrentView = viewName;
        Navigated?.Invoke(this, new NavigationEventArgs(viewName, parameter));
    }

    public void NavigateBack()
    {
        if (!CanNavigateBack) return;
        _navigationStack.Pop();
        var viewName = _navigationStack.Peek();
        CurrentView = viewName;
        Navigated?.Invoke(this, new NavigationEventArgs(viewName));
    }
}

public interface IDialogService
{
    Task<bool> ConfirmAsync(string message, string title = "تأیید");
    Task ShowErrorAsync(string message, string title = "خطا");
    Task ShowSuccessAsync(string message, string title = "موفق");
    Task ShowInfoAsync(string message, string title = "اطلاعات");
    Task ShowWarningAsync(string message, string title = "هشدار");
    Task<string?> ShowInputAsync(string prompt, string title = "ورود اطلاعات");
    Task<string?> PromptAsync(string message, string title = "ورودی", string defaultValue = "");
}

public class DialogService : IDialogService
{
    public Task<bool> ConfirmAsync(string message, string title = "تأیید")
    {
        var result = System.Windows.MessageBox.Show(message, title,
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Question,
            System.Windows.MessageBoxResult.No,
            System.Windows.MessageBoxOptions.RightAlign | System.Windows.MessageBoxOptions.RtlReading);

        return Task.FromResult(result == System.Windows.MessageBoxResult.Yes);
    }

    public Task ShowErrorAsync(string message, string title = "خطا")
    {
        System.Windows.MessageBox.Show(message, title,
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Error,
            System.Windows.MessageBoxResult.OK,
            System.Windows.MessageBoxOptions.RightAlign | System.Windows.MessageBoxOptions.RtlReading);
        return Task.CompletedTask;
    }

    public Task ShowSuccessAsync(string message, string title = "موفق")
    {
        System.Windows.MessageBox.Show(message, title,
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Information,
            System.Windows.MessageBoxResult.OK,
            System.Windows.MessageBoxOptions.RightAlign | System.Windows.MessageBoxOptions.RtlReading);
        return Task.CompletedTask;
    }

    public Task ShowInfoAsync(string message, string title = "اطلاعات")
    {
        System.Windows.MessageBox.Show(message, title,
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Information,
            System.Windows.MessageBoxResult.OK,
            System.Windows.MessageBoxOptions.RightAlign | System.Windows.MessageBoxOptions.RtlReading);
        return Task.CompletedTask;
    }

    public Task ShowWarningAsync(string message, string title = "هشدار")
    {
        System.Windows.MessageBox.Show(message, title,
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Warning,
            System.Windows.MessageBoxResult.OK,
            System.Windows.MessageBoxOptions.RightAlign | System.Windows.MessageBoxOptions.RtlReading);
        return Task.CompletedTask;
    }

    public Task<string?> ShowInputAsync(string prompt, string title = "ورود اطلاعات")
    {
        // Simple implementation - in production use a custom dialog
        return Task.FromResult<string?>(null);
    }

    public Task<string?> PromptAsync(string message, string title = "ورودی", string defaultValue = "")
        => Task.FromResult<string?>(defaultValue);
}

public interface IThemeService
{
    string CurrentTheme { get; }
    void SetTheme(string theme);
    void ToggleTheme();
}

public class ThemeService : IThemeService
{
    public string CurrentTheme { get; private set; } = "Dark";

    public void SetTheme(string theme)
    {
        CurrentTheme = theme;
        var dict = new System.Windows.ResourceDictionary
        {
            Source = new Uri($"/Assets/Themes/{theme}.xaml", UriKind.Relative)
        };

        var mergedDicts = System.Windows.Application.Current.Resources.MergedDictionaries;
        mergedDicts.Clear();
        mergedDicts.Add(dict);
    }

    public void ToggleTheme()
    {
        SetTheme(CurrentTheme == "Dark" ? "Light" : "Dark");
    }
}
