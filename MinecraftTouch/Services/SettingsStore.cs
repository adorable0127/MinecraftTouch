using System.Runtime.Serialization.Json;
using System.Windows;
using MinecraftTouch.Models;

namespace MinecraftTouch.Services;

internal static class JsonStorage
{
    public static T? Read<T>(string path) where T : class
    {
        using var stream = File.OpenRead(path);
        return new DataContractJsonSerializer(typeof(T)).ReadObject(stream) as T;
    }
    public static void Write<T>(string path, T value)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var temp = path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            using (var stream = File.Create(temp)) new DataContractJsonSerializer(typeof(T)).WriteObject(stream, value);
            if (File.Exists(path)) File.Replace(temp, path, null);
            else File.Move(temp, path);
        }
        finally { if (File.Exists(temp)) File.Delete(temp); }
    }
}

internal static class SettingsStore
{
    public static string DataDirectory { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "xztx127", "MinecraftTouch");
    public static ToolSettings Current { get; private set; } = new();
    public static string? LastSaveError { get; private set; }
    public static void Load()
    {
        try
        {
            var path = Path.Combine(DataDirectory, "settings.json");
            if (File.Exists(path)) Current = JsonStorage.Read<ToolSettings>(path) ?? new ToolSettings();
        }
        catch (Exception ex) { AppLog.Write("读取配置失败，使用默认配置：" + ex); }
        Current.TouchOverlayButtonScale = Normalize(Current.TouchOverlayButtonScale, 0.5, 2, 1);
        Current.TouchOverlayOpacityPercent = Normalize(Current.TouchOverlayOpacityPercent, 20, 100, 85);
        Current.TouchOverlayLookSensitivity = Normalize(Current.TouchOverlayLookSensitivity, 0.4, 4, 1.4);
        Current.ActiveSeconds = Math.Max(0, Compatibility.IsFinite(Current.ActiveSeconds) ? Current.ActiveSeconds : 0);
        Current.SuccessfulStarts = Math.Max(0, Current.SuccessfulStarts);
    }
    private static double Normalize(double n, double min, double max, double fallback)
        => Compatibility.IsFinite(n) && n >= min && n <= max ? n : fallback;
    public static bool TrySave()
    {
        try { JsonStorage.Write(Path.Combine(DataDirectory, "settings.json"), Current); LastSaveError = null; return true; }
        catch (Exception ex) { LastSaveError = ex.Message; AppLog.Write("保存配置失败：" + ex); return false; }
    }
}

internal static class AppLog
{
    private static readonly object Sync = new();
    public static void Write(string message)
    {
        try
        {
            lock (Sync)
            {
                Directory.CreateDirectory(SettingsStore.DataDirectory);
                var path = Path.Combine(SettingsStore.DataDirectory, "tool.log");
                if (File.Exists(path) && new FileInfo(path).Length > 2 * 1024 * 1024) File.Delete(path);
                File.AppendAllText(path, DateTime.Now.ToString("s") + " " + message + Environment.NewLine);
            }
        }
        catch { /* Logging must never interrupt input cleanup. */ }
    }
    public static void Warn(string message)
    {
        Write(message);
        MessageBox.Show(message, App.DisplayName, MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
