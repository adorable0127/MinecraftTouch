using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MinecraftTouch.Models;
using MinecraftTouch.Services;

namespace MinecraftTouch.Views;

public sealed class WindowRow : INotifyPropertyChanged
{
    public GameProcessInfo Info { get; }
    public string Title => Info.VersionId;
    public string Detail => $"{Info.ProcessName}  ·  PID {Info.Pid}";
    public string? Error => TouchOverlayService.GetStatus(Info).Error;
    public string Status => TouchOverlayService.GetStatus(Info).State switch
    {
        TouchOverlayState.Starting => "正在开启…", TouchOverlayState.Enabled => "已开启",
        TouchOverlayState.Failed => "开启失败", _ => "未开启"
    };
    public WindowRow(GameProcessInfo info) => Info = info;
    public event PropertyChangedEventHandler? PropertyChanged;
    public void Refresh() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
}

public partial class MainWindow : Window
{
    private readonly List<WindowRow> _known = new();
    private readonly ObservableCollection<WindowRow> _visible = new();
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(2) };
    private readonly Stopwatch _usageClock = Stopwatch.StartNew();
    private double _lastUsageSample;
    private double _unsavedSeconds;
    private bool _scanInProgress;
    private bool _closing;
    private bool _ready;

    public MainWindow()
    {
        InitializeComponent();
        Width = Math.Min(Width, SystemParameters.WorkArea.Width);
        Height = Math.Min(Height, SystemParameters.WorkArea.Height);
        WindowList.ItemsSource = _visible;
        var cfg = SettingsStore.Current;
        ScaleSlider.Value = cfg.TouchOverlayButtonScale;
        OpacitySlider.Value = cfg.TouchOverlayOpacityPercent;
        SensitivitySlider.Value = cfg.TouchOverlayLookSensitivity;
        ScaleSlider.ValueChanged += Parameters_Changed;
        OpacitySlider.ValueChanged += Parameters_Changed;
        SensitivitySlider.ValueChanged += Parameters_Changed;
        UpdateThemeButton();
        TouchOverlayService.Changed += Overlay_Changed;
        _timer.Tick += async (_, _) => { CountUsage(); await RefreshAsync(); };
        Loaded += async (_, _) => { _timer.Start(); await RefreshAsync(); };
        Closing += OnClosing;
        _ready = true;
        UpdateRows();
    }
    private void UpdateThemeButton() => ThemeButton.Content = SettingsStore.Current.DarkMode ? "切换浅色" : "切换深色";
    private void Theme_Click(object sender, RoutedEventArgs e)
    {
        SettingsStore.Current.DarkMode = !SettingsStore.Current.DarkMode;
        ThemeService.Apply(SettingsStore.Current.DarkMode);
        UpdateThemeButton(); SavePreferences();
    }
    private void Parameters_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!_ready) return;
        var cfg = SettingsStore.Current;
        cfg.TouchOverlayButtonScale = ScaleSlider.Value;
        cfg.TouchOverlayOpacityPercent = OpacitySlider.Value;
        cfg.TouchOverlayLookSensitivity = SensitivitySlider.Value;
        SavePreferences();
    }
    private void SavePreferences()
    {
        StatusText.Text = SettingsStore.TrySave()
            ? "设置已保存。触摸板参数在下次开启时生效；按键与布局保存后立即生效。"
            : "设置保存失败：" + SettingsStore.LastSaveError;
    }
    private async void Refresh_Click(object sender, RoutedEventArgs e) => await RefreshAsync();
    private async Task RefreshAsync()
    {
        if (_scanInProgress || _closing) return;
        _scanInProgress = true;
        try
        {
            var candidates = await Task.Run(GameWindowDiscovery.Scan);
            if (_closing) return;
            foreach (var row in _known.Where(r => r.Info.HasExited).ToList())
            {
                TouchOverlayService.Detach(row.Info);
                _known.Remove(row); _visible.Remove(row); row.Info.Dispose();
            }
            foreach (var candidate in candidates)
            {
                var row = _known.FirstOrDefault(r => r.Info.Pid == candidate.Pid && r.Info.WindowHandle == candidate.Handle);
                if (row != null)
                {
                    row.Info.VersionId = candidate.Title;
                    row.Info.IsLikelyMinecraft = candidate.Likely;
                }
                else
                {
                    try { _known.Add(new WindowRow(new GameProcessInfo(candidate.Pid, candidate.Handle, candidate.Title, candidate.ProcessName, candidate.Likely))); }
                    catch { /* Process closed between scan and merge. */ }
                }
            }
            UpdateRows();
        }
        catch (Exception ex) { AppLog.Write(ex.ToString()); StatusText.Text = "刷新窗口失败，可点击刷新重试。"; }
        finally { _scanInProgress = false; }
    }
    private void Filter_Changed(object sender, RoutedEventArgs e) { if (_ready) UpdateRows(); }
    private void Selection_Changed(object sender, SelectionChangedEventArgs e) { if (_ready) RefreshActions(); }
    private void Overlay_Changed() { if (!_closing && _ready) UpdateRows(); }
    private void UpdateRows()
    {
        if (!_ready) return;
        var shown = _known.Where(r => !r.Info.HasExited && (ShowAllCheck.IsChecked == true || r.Info.IsLikelyMinecraft ||
            TouchOverlayService.GetStatus(r.Info).State is TouchOverlayState.Enabled or TouchOverlayState.Starting)).ToList();
        foreach (var row in _visible.Where(r => !shown.Contains(r)).ToList()) _visible.Remove(row);
        foreach (var row in shown)
        {
            if (!_visible.Contains(row)) _visible.Add(row);
            row.Refresh();
        }
        if (WindowList.SelectedItem == null && _visible.Count == 1) WindowList.SelectedIndex = 0;
        EmptyHint.Visibility = _visible.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        var enabled = _known.Count(r => TouchOverlayService.GetStatus(r.Info).State == TouchOverlayState.Enabled);
        SummaryText.Text = $"可选窗口 {_visible.Count} 个  ·  触摸板已开启 {enabled} 个";
        RecommendationBanner.Visibility = SettingsStore.Current.ShouldRecommend(DateTime.UtcNow) ? Visibility.Visible : Visibility.Collapsed;
        RefreshActions();
    }
    private void RefreshActions()
    {
        var row = WindowList.SelectedItem as WindowRow;
        var running = row != null && !row.Info.HasExited;
        var state = row == null ? TouchOverlayState.Disabled : TouchOverlayService.GetStatus(row.Info).State;
        EnableButton.IsEnabled = running && (state is TouchOverlayState.Disabled or TouchOverlayState.Failed);
        DisableButton.IsEnabled = running && (state is TouchOverlayState.Enabled or TouchOverlayState.Starting);
        DisableButton.Content = state == TouchOverlayState.Starting ? "取消开启" : "关闭触摸板";
        FocusButton.IsEnabled = running;
    }
    private void Enable_Click(object sender, RoutedEventArgs e)
    {
        if (WindowList.SelectedItem is not WindowRow row || row.Info.HasExited) return;
        try
        {
            TouchOverlayService.Attach(row.Info, SettingsStore.Current);
            GameWindowDiscovery.Focus(row.Info.WindowHandle);
            StatusText.Text = "已请求开启。若按键无响应，请确认工具与游戏权限一致，并使用窗口或无边框模式。";
        }
        catch (Exception ex) { AppLog.Warn("触摸板开启失败：" + ex.Message); }
        UpdateRows();
    }
    private void Disable_Click(object sender, RoutedEventArgs e)
    {
        if (WindowList.SelectedItem is WindowRow row) TouchOverlayService.Detach(row.Info);
        UpdateRows();
    }
    private void Focus_Click(object sender, RoutedEventArgs e)
    {
        if (WindowList.SelectedItem is WindowRow row && !row.Info.HasExited) GameWindowDiscovery.Focus(row.Info.WindowHandle);
    }
    private void Layout_Click(object sender, RoutedEventArgs e) => new TouchControlsSettingsWindow { Owner = this }.ShowDialog();
    private void CountUsage()
    {
        var now = _usageClock.Elapsed.TotalSeconds;
        var elapsed = Math.Min(3, Math.Max(0, now - _lastUsageSample));
        _lastUsageSample = now;
        var foreground = GameWindowDiscovery.GetForegroundWindow();
        if (!_known.Any(r => r.Info.WindowHandle == foreground && TouchOverlayService.GetStatus(r.Info).State == TouchOverlayState.Enabled)) return;
        SettingsStore.Current.ActiveSeconds += elapsed;
        _unsavedSeconds += elapsed;
        if (_unsavedSeconds >= 30) { SettingsStore.TrySave(); _unsavedSeconds = 0; }
    }
    private void MoreFeatures_Click(object sender, RoutedEventArgs e)
    {
        if (MessageBox.Show(this, "作者的启动器有更高级的功能\n\n是否打开 XCL 启动器网站？\nhttps://xcl.xztx127.dpdns.org",
                "使用更多功能", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            OpenLauncherWebsite();
    }
    private void Download_Click(object sender, RoutedEventArgs e) => OpenLauncherWebsite();
    private void OpenLauncherWebsite()
    {
        try { Process.Start(new ProcessStartInfo("https://xcl.xztx127.dpdns.org") { UseShellExecute = true }); }
        catch (Exception ex) { AppLog.Warn("无法打开浏览器，请手动访问 https://xcl.xztx127.dpdns.org\n" + ex.Message); }
    }
    private void Snooze_Click(object sender, RoutedEventArgs e)
    {
        SettingsStore.Current.RecommendationSnoozedUntilUtcTicks = DateTime.UtcNow.AddDays(7).Ticks;
        SavePreferences(); UpdateRows();
    }
    private void Dismiss_Click(object sender, RoutedEventArgs e)
    {
        SettingsStore.Current.RecommendationDismissed = true;
        SavePreferences(); UpdateRows();
    }
    private void OnClosing(object? sender, CancelEventArgs e)
    {
        CountUsage(); _closing = true; _timer.Stop();
        TouchOverlayService.Changed -= Overlay_Changed;
        TouchOverlayService.CloseAll();
        TouchMousePromotionFilter.Disable();
        SettingsStore.TrySave();
        foreach (var row in _known) row.Info.Dispose();
        _known.Clear();
    }
}
