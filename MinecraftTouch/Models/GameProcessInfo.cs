using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MinecraftTouch.Models;

public sealed class GameProcessInfo : IDisposable
{
    [DllImport("user32.dll")] private static extern bool IsWindow(IntPtr hwnd);
    public Process Process { get; }
    public int Pid { get; }
    public IntPtr WindowHandle { get; }
    public string VersionId { get; set; }
    public string ProcessName { get; }
    public bool IsLikelyMinecraft { get; set; }
    public bool HasExited
    {
        get { try { return Process.HasExited || !IsWindow(WindowHandle); } catch { return true; } }
    }
    public GameProcessInfo(int pid, IntPtr hwnd, string title, string processName, bool likely)
    {
        Pid = pid; WindowHandle = hwnd; VersionId = title; ProcessName = processName;
        IsLikelyMinecraft = likely;
        Process = System.Diagnostics.Process.GetProcessById(pid);
        try { Process.EnableRaisingEvents = true; } catch { /* Timer also detects process exit. */ }
    }
    public void Dispose() => Process.Dispose();
}
