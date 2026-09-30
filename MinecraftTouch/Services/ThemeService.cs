using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MinecraftTouch.Services;

internal static class ThemeService
{
    public static void Apply(bool dark)
    {
        var iconName = dark ? "app-dark.ico" : "app.ico";
        var iconUri = new Uri("pack://application:,,,/MinecraftTouch;component/Resources/" + iconName);
        // 窗口/任务栏图标：保留完整 ico 解码器，让系统按需要的尺寸选帧。
        var icon = BitmapFrame.Create(iconUri);
        icon.Freeze();
        Application.Current.Resources["ToolIcon"] = icon;
        // 界面内 40x40 的大图标：取 ico 中最大的一帧，避免只拿到第一帧 16x16 而发糊。
        var decoder = new IconBitmapDecoder(iconUri, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
        var large = decoder.Frames[0];
        foreach (var frame in decoder.Frames)
            if (frame.PixelWidth > large.PixelWidth) large = frame;
        large.Freeze();
        Application.Current.Resources["ToolIconLarge"] = large;
        void Set(string key, string value) => Application.Current.Resources[key] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));
        Set("SideBrush", dark ? "#111827" : "#F3F6FC");
        Set("PanelBrush", dark ? "#1E293B" : "#FFFFFF");
        Set("TextPrimaryBrush", dark ? "#F1F5F9" : "#172033");
        Set("TextSecondaryBrush", dark ? "#CBD5E1" : "#52627A");
        Set("DividerBrush", dark ? "#475569" : "#CBD5E1");
        Set("BorderBrush2", dark ? "#64748B" : "#94A3B8");
        Set("BorderHoverBrush", dark ? "#93C5FD" : "#2563EB");
        Set("AccentBrush", dark ? "#93C5FD" : "#1D4ED8");
        Set("DangerBrush", dark ? "#FCA5A5" : "#B91C1C");
        Set("HoverBrush", dark ? "#334155" : "#E2E8F0");
    }
}
