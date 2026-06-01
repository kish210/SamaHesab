using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SamaHesab.WPF.Services;

namespace SamaHesab.WPF.Views.Shell;

/// <summary>
/// Lightweight, code-only dialog to view/edit/test the database connection string.
/// Opened from the login window's "تنظیمات" (Settings) button.
/// </summary>
public class ConnectionSettingsWindow : Window
{
    private readonly TextBox _txtConnection;
    private readonly TextBlock _status;

    public ConnectionSettingsWindow()
    {
        Title = "تنظیمات اتصال به پایگاه داده";
        Width = 640;
        Height = 380;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        FlowDirection = FlowDirection.RightToLeft;
        Background = new SolidColorBrush(Color.FromRgb(0x1E, 0x29, 0x3B));
        ResizeMode = ResizeMode.NoResize;

        var root = new StackPanel { Margin = new Thickness(24) };

        root.Children.Add(new TextBlock
        {
            Text = "رشته اتصال (Connection String)",
            Foreground = Brushes.White,
            FontSize = 16,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 0, 0, 8)
        });

        root.Children.Add(new TextBlock
        {
            Text = "نمونه تک‌کاربره (SQL Express):\n" +
                   "Server=.\\SQLEXPRESS;Database=SamaHesab;Trusted_Connection=True;TrustServerCertificate=True;\n\n" +
                   "نمونه تحت شبکه (SQL Server):\n" +
                   "Server=192.168.1.10;Database=SamaHesab;User Id=sa;Password=***;TrustServerCertificate=True;",
            Foreground = new SolidColorBrush(Color.FromRgb(0x9C, 0xA8, 0xB8)),
            FontSize = 12,
            Margin = new Thickness(0, 0, 0, 12),
            TextWrapping = TextWrapping.Wrap
        });

        _txtConnection = new TextBox
        {
            Text = AppSettingsStore.GetConnectionString(),
            MinHeight = 70,
            FontSize = 13,
            Padding = new Thickness(8),
            TextWrapping = TextWrapping.Wrap,
            AcceptsReturn = true,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };
        root.Children.Add(_txtConnection);

        _status = new TextBlock
        {
            Margin = new Thickness(0, 12, 0, 12),
            FontSize = 13,
            TextWrapping = TextWrapping.Wrap,
            Foreground = Brushes.White
        };
        root.Children.Add(_status);

        var buttons = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Left
        };

        var btnTest = MakeButton("آزمایش اتصال", Color.FromRgb(0x25, 0x63, 0xEB));
        btnTest.Click += async (_, _) => await TestAsync();
        buttons.Children.Add(btnTest);

        var btnSave = MakeButton("ذخیره", Color.FromRgb(0x16, 0xA3, 0x4A));
        btnSave.Click += (_, _) => Save();
        buttons.Children.Add(btnSave);

        var btnCancel = MakeButton("انصراف", Color.FromRgb(0x4B, 0x55, 0x63));
        btnCancel.Click += (_, _) => Close();
        buttons.Children.Add(btnCancel);

        root.Children.Add(buttons);
        Content = root;
    }

    private static Button MakeButton(string text, Color color) => new()
    {
        Content = text,
        Margin = new Thickness(0, 0, 10, 0),
        Padding = new Thickness(18, 8, 18, 8),
        FontSize = 14,
        Foreground = Brushes.White,
        Background = new SolidColorBrush(color),
        BorderThickness = new Thickness(0),
        Cursor = System.Windows.Input.Cursors.Hand
    };

    private async Task TestAsync()
    {
        _status.Foreground = Brushes.Khaki;
        _status.Text = "در حال آزمایش اتصال...";
        var cs = _txtConnection.Text.Trim();
        try
        {
            await Task.Run(() =>
            {
                using var conn = new Microsoft.Data.SqlClient.SqlConnection(cs);
                conn.Open();
            });
            _status.Foreground = Brushes.LightGreen;
            _status.Text = "✅ اتصال با موفقیت برقرار شد.";
        }
        catch (Exception ex)
        {
            _status.Foreground = Brushes.IndianRed;
            _status.Text = "❌ خطا در اتصال: " + ex.Message;
        }
    }

    private void Save()
    {
        try
        {
            AppSettingsStore.SaveConnectionString(_txtConnection.Text.Trim());
            MessageBox.Show(
                "تنظیمات ذخیره شد. برای اعمال تغییرات برنامه را دوباره اجرا کنید.",
                "ذخیره شد", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show("خطا در ذخیره تنظیمات: " + ex.Message,
                "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
