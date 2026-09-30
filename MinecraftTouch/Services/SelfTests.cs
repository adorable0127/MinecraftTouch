using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MinecraftTouch.Models;
using MinecraftTouch.Views;

namespace MinecraftTouch.Services;

internal static class SelfTests
{
    // Run only when explicitly launched with --self-test. Uses an isolated temporary profile.
    public static int Run()
    {
        var profile = Path.Combine(Path.GetTempPath(), "MinecraftTouch.Tests." + Guid.NewGuid().ToString("N"));
        var oldDirectory = SettingsStore.DataDirectory;
        var report = new List<string>();
        MainWindow? main = null;
        TouchControlsSettingsWindow? editor = null;
        TouchOverlayWindow? overlay = null;
        Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        void Check(bool success, string name)
        {
            if (!success) throw new InvalidOperationException(name);
            report.Add("PASS " + name);
        }
        try
        {
            SettingsStore.DataDirectory = profile;
            SettingsStore.Load();
            var controls = TouchControlSettingsService.CreateDefaults();
            Check(controls.Count >= 30 && TouchControlSettingsService.Validate(controls) == null, "default controls valid");
            Check(controls.Single(c => c.Id == "inventory").Visibility == TouchControlVisibility.Game,
                "inventory key defaults to game-only visibility");
            var legacy = controls.Select(c => c.Copy()).ToList();
            var inventory = legacy.Single(c => c.Id == "inventory");
            inventory.Visibility = TouchControlVisibility.Always;
            inventory.Binding = "R";
            inventory.X = 123;
            Check(TouchControlSettingsService.UpgradeInventoryVisibility(legacy) &&
                inventory.Visibility == TouchControlVisibility.Game && inventory.Binding == "R" && inventory.X == 123 &&
                legacy.Single(c => c.Id == "escape").Visibility == TouchControlVisibility.Always,
                "migrate only inventory visibility while preserving user layout");
            Check(!TouchControlSettingsService.UpgradeInventoryVisibility(legacy), "inventory migration is idempotent");
            TouchControlSettingsService.Save(controls);
            var loaded = TouchControlSettingsService.Load();
            Check(loaded.Count == controls.Count && loaded[0].Id == controls[0].Id && loaded.All(c => c.Enabled), "JSON control round trip");
            loaded[0].Width = double.NaN;
            Check(TouchControlSettingsService.Validate(loaded) != null, "reject non-finite layout");
            loaded = TouchControlSettingsService.CreateDefaults();
            loaded[1].Id = loaded[0].Id;
            Check(TouchControlSettingsService.Validate(loaded) != null, "reject duplicate IDs");
            Check(TouchInputInjector.IsValidBinding(" CTRL + A ") && TouchInputInjector.IsValidBinding("F24") &&
                !TouchInputInjector.IsValidBinding("CTRL+NO_SUCH_KEY"), "key binding validation");
            Check(Compatibility.Clamp(-2, 0, 4) == 0 && Compatibility.Clamp(5L, 0L, 4L) == 4, "Framework compatibility helpers");
            var utc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var cfg = new ToolSettings();
            Check(!cfg.ShouldRecommend(utc), "no recommendation on first use");
            cfg.SuccessfulStarts = 5;
            Check(cfg.ShouldRecommend(utc), "recommendation after five successful starts");
            cfg.SuccessfulStarts = 0; cfg.ActiveSeconds = 7200;
            Check(cfg.ShouldRecommend(utc), "recommendation after two hours");
            cfg.RecommendationSnoozedUntilUtcTicks = utc.AddDays(7).Ticks;
            Check(!cfg.ShouldRecommend(utc) && cfg.ShouldRecommend(utc.AddDays(7)), "seven-day snooze");
            cfg.RecommendationDismissed = true;
            Check(!cfg.ShouldRecommend(utc.AddDays(8)), "permanent dismissal");
            SettingsStore.Current.DarkMode = false;
            SettingsStore.Current.RecommendationDismissed = true;
            Check(SettingsStore.TrySave(), "save preferences");
            SettingsStore.Load();
            Check(!SettingsStore.Current.DarkMode && SettingsStore.Current.RecommendationDismissed, "preference round trip");
            foreach (var dark in new[] { true, false })
            {
                ThemeService.Apply(dark);
                var brush = (SolidColorBrush)Application.Current.FindResource("TextPrimaryBrush");
                Check(brush.Color.A == 255, "opaque theme text " + dark);
                Check(Application.Current.FindResource("ToolIcon") is System.Windows.Media.Imaging.BitmapFrame, "theme icon loads " + dark);
            }
            main = new MainWindow();
            main.Show(); main.UpdateLayout();
            Check(main.Title == App.DisplayName, "standalone main window and app name");
            editor = new TouchControlsSettingsWindow { Owner = main };
            editor.Show(); editor.UpdateLayout();
            var grid = Find<DataGrid>(editor)!;
            Check(grid != null && grid.Columns.Count == 12, "settings editor columns and resource dictionary");
            grid!.SelectedIndex = 0;
            grid.ScrollIntoView(grid.SelectedItem);
            grid.UpdateLayout();
            var cellText = grid.Columns[1].GetCellContent(grid.SelectedItem) as TextBlock;
            Check(cellText != null && ((SolidColorBrush)cellText.Foreground).Color == Colors.White, "selected row foreground");
            var checkbox = grid.Columns[0].GetCellContent(grid.SelectedItem) as CheckBox;
            checkbox?.ApplyTemplate();
            Check(checkbox != null && checkbox.Template.FindName("box", checkbox) is Border,
                "rounded checkbox template");
            var combo = grid.Columns[3].GetCellContent(grid.SelectedItem) as ComboBox;
            Check(combo != null && combo.Template != null, "enum display template");
            overlay = new TouchOverlayWindow(IntPtr.Zero, 1, 85, 1.4);
            Check(overlay.FindName("ControlCanvas") is Canvas, "overlay XAML loads without launcher");
            report.Add("PASS all checks. Native touch/game interaction still requires manual Windows testing.");
            return 0;
        }
        catch (Exception ex) { report.Add("FAIL " + ex); return 1; }
        finally
        {
            overlay?.ShutdownOverlay();
            editor?.Close(); main?.Close();
            SettingsStore.DataDirectory = oldDirectory;
            try { Directory.Delete(profile, true); } catch { }
            File.WriteAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "self-test-result.txt"), report);
        }
    }
    private static T? Find<T>(DependencyObject root) where T : DependencyObject
    {
        if (root is T value) return value;
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
        {
            var found = Find<T>(VisualTreeHelper.GetChild(root, i));
            if (found != null) return found;
        }
        return null;
    }
}
