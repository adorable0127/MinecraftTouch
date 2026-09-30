namespace MinecraftTouch.Models;

public sealed class ToolSettings
{
    public int TouchLayoutRevision { get; set; }
    public bool DarkMode { get; set; } = true;
    public double TouchOverlayButtonScale { get; set; } = 1.0;
    public double TouchOverlayOpacityPercent { get; set; } = 85;
    public double TouchOverlayLookSensitivity { get; set; } = 1.4;
    public int SuccessfulStarts { get; set; }
    public double ActiveSeconds { get; set; }
    public bool RecommendationDismissed { get; set; }
    public long RecommendationSnoozedUntilUtcTicks { get; set; }
    public bool ShouldRecommend(DateTime utcNow) => !RecommendationDismissed &&
        utcNow.Ticks >= RecommendationSnoozedUntilUtcTicks &&
        (SuccessfulStarts >= 5 || ActiveSeconds >= 7200);
}
