using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace MinecraftTouch.Services;

internal static class GameWindowDiscovery
{
    internal sealed record Candidate(int Pid, IntPtr Handle, string Title, string ProcessName, bool Likely);
    private delegate bool EnumProc(IntPtr hwnd, IntPtr state);
    [DllImport("user32.dll")] private static extern bool EnumWindows(EnumProc callback, IntPtr state);
    [DllImport("user32.dll")] private static extern bool IsWindowVisible(IntPtr hwnd);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetWindowText(IntPtr hwnd, StringBuilder text, int count);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetClassName(IntPtr hwnd, StringBuilder text, int count);
    [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint pid);
    [DllImport("user32.dll")] public static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hwnd);
    [DllImport("user32.dll")] private static extern bool IsIconic(IntPtr hwnd);
    [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr hwnd, int command);
    public static void Focus(IntPtr hwnd)
    {
        if (IsIconic(hwnd)) ShowWindow(hwnd, 9);
        SetForegroundWindow(hwnd);
    }
    public static List<Candidate> Scan()
    {
        var result = new List<Candidate>();
        using var self = Process.GetCurrentProcess();
        var ownPid = self.Id;
        EnumWindows((hwnd, _) =>
        {
            try
            {
                if (!IsWindowVisible(hwnd)) return true;
                GetWindowThreadProcessId(hwnd, out var pid);
                if (pid == 0 || pid == ownPid) return true;
                var title = new StringBuilder(1024);
                GetWindowText(hwnd, title, title.Capacity);
                if (title.Length == 0) return true;
                var windowClass = new StringBuilder(256);
                GetClassName(hwnd, windowClass, windowClass.Capacity);
                using var process = Process.GetProcessById((int)pid);
                var name = process.ProcessName;
                var java = name.Equals("java", StringComparison.OrdinalIgnoreCase) || name.Equals("javaw", StringComparison.OrdinalIgnoreCase);
                var likely = java && (title.ToString().IndexOf("minecraft", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    windowClass.ToString().Equals("GLFW30", StringComparison.OrdinalIgnoreCase) ||
                    windowClass.ToString().StartsWith("LWJGL", StringComparison.OrdinalIgnoreCase));
                result.Add(new Candidate((int)pid, hwnd, title.ToString(), name, likely));
            }
            catch { /* A window may disappear or deny process access during enumeration. */ }
            return true;
        }, IntPtr.Zero);
        return result;
    }
}
