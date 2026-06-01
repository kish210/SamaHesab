using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace SamaHesab.WPF.Controls;

public partial class PersianDatePicker : UserControl
{
    private static readonly PersianCalendar _persianCalendar = new();

    public static readonly DependencyProperty PersianDateProperty =
        DependencyProperty.Register(nameof(PersianDate), typeof(string), typeof(PersianDatePicker),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnPersianDateChanged));

    public string PersianDate
    {
        get => (string)GetValue(PersianDateProperty);
        set => SetValue(PersianDateProperty, value);
    }

    public static readonly DependencyProperty SelectedDateProperty =
        DependencyProperty.Register(nameof(SelectedDate), typeof(DateTime?), typeof(PersianDatePicker),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public DateTime? SelectedDate
    {
        get => (DateTime?)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    public PersianDatePicker()
    {
        InitializeComponent();
        // Set today's date as default
        PersianDate = ToPersian(DateTime.Today);
    }

    private static void OnPersianDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PersianDatePicker picker && e.NewValue is string dateStr)
        {
            if (TryParseToGregorian(dateStr, out var gregorian))
                picker.SelectedDate = gregorian;
        }
    }

    private static string ToPersian(DateTime date)
    {
        int year = _persianCalendar.GetYear(date);
        int month = _persianCalendar.GetMonth(date);
        int day = _persianCalendar.GetDayOfMonth(date);
        return $"{year:D4}/{month:D2}/{day:D2}";
    }

    private static bool TryParseToGregorian(string persianDate, out DateTime result)
    {
        result = DateTime.MinValue;
        var parts = persianDate?.Split('/');
        if (parts?.Length != 3) return false;

        if (int.TryParse(parts[0], out int year) &&
            int.TryParse(parts[1], out int month) &&
            int.TryParse(parts[2], out int day))
        {
            try
            {
                result = _persianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
                return true;
            }
            catch { return false; }
        }
        return false;
    }

    private void CalendarButton_Click(object sender, RoutedEventArgs e)
    {
        // Open Persian Calendar Popup (simplified)
        PersianDate = ToPersian(DateTime.Today);
    }
}
