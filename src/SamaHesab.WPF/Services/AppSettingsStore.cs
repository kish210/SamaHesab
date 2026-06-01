using System.IO;
using System.Text.Json;

namespace SamaHesab.WPF.Services;

/// <summary>
/// Stores user-editable settings (connection string) in a writable location
/// under %AppData%\SamaHesab so the app works even when installed to Program Files.
/// </summary>
public static class AppSettingsStore
{
    public static string AppDataDir { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SamaHesab");

    public static string FilePath => Path.Combine(AppDataDir, "settings.user.json");
    public static string LogDirectory => Path.Combine(AppDataDir, "logs");

    // Default points to a local SQL Server Express instance (Windows auth).
    // Change it from the login screen's "تنظیمات اتصال" if your server differs.
    public const string DefaultConnectionString =
        "Server=.\\SQLEXPRESS;Database=SamaHesab;Trusted_Connection=True;" +
        "TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=True;Connect Timeout=5;";

    private class Model
    {
        public Dictionary<string, string> ConnectionStrings { get; set; } = new();
    }

    /// <summary>Make sure the directories and a default settings file exist.</summary>
    public static void EnsureInitialized()
    {
        Directory.CreateDirectory(AppDataDir);
        Directory.CreateDirectory(LogDirectory);
        if (!File.Exists(FilePath))
            SaveConnectionString(DefaultConnectionString);
    }

    public static string GetConnectionString()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                var model = JsonSerializer.Deserialize<Model>(File.ReadAllText(FilePath));
                if (model?.ConnectionStrings?.TryGetValue("DefaultConnection", out var cs) == true
                    && !string.IsNullOrWhiteSpace(cs))
                    return cs;
            }
        }
        catch { /* fall back to default */ }
        return DefaultConnectionString;
    }

    public static void SaveConnectionString(string connectionString)
    {
        Directory.CreateDirectory(AppDataDir);
        var model = new Model
        {
            ConnectionStrings = new Dictionary<string, string>
            {
                ["DefaultConnection"] = connectionString
            }
        };
        File.WriteAllText(FilePath,
            JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true }));
    }
}
