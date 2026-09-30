using System.Diagnostics;

namespace MinecraftTouch.Services
{
    internal static class Compatibility
    {
        private static readonly Stopwatch Clock = Stopwatch.StartNew();
        public static long TickCount64 => Clock.ElapsedMilliseconds;
        public static bool IsFinite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
        public static double Clamp(double value, double min, double max) => Math.Max(min, Math.Min(max, value));
        public static int Clamp(int value, int min, int max) => Math.Max(min, Math.Min(max, value));
        public static long Clamp(long value, long min, long max) => Math.Max(min, Math.Min(max, value));
        public static bool Remove<TKey, TValue>(this Dictionary<TKey, TValue> values, TKey key, out TValue value)
        {
            if (!values.TryGetValue(key, out value!)) return false;
            return values.Remove(key);
        }
    }
}
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
