using System.Threading;
using System.Windows;
using MinecraftTouch.Services;
using MinecraftTouch.Views;

namespace MinecraftTouch;

public partial class App : Application
{
    public const string DisplayName = "我的世界java版触屏工具 --by xztx127";
    public const string Version = "1.0.0";
    private Mutex? _singleInstance;
    private bool _ownsMutex;
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        if (e.Args.Contains("--self-test"))
        {
            Shutdown(SelfTests.Run());
            return;
        }
        _singleInstance = new Mutex(true, "Local\\xztx127.MinecraftTouch.1", out _ownsMutex);
        if (!_ownsMutex)
        {
            MessageBox.Show("工具已经在运行，请切换到已打开的主界面。", DisplayName);
            Shutdown(); return;
        }
        SettingsStore.Load();
        ThemeService.Apply(SettingsStore.Current.DarkMode);
        MainWindow = new MainWindow();
        MainWindow.Show();
    }
    protected override void OnExit(ExitEventArgs e)
    {
        TouchOverlayService.CloseAll();
        TouchMousePromotionFilter.Disable();
        if (_ownsMutex)
        {
            SettingsStore.TrySave();
            _singleInstance?.ReleaseMutex();
        }
        _singleInstance?.Dispose();
        base.OnExit(e);
    }
}
