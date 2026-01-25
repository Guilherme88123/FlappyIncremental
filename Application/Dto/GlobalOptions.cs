using System.Numerics;

namespace Application.Dto;

public static class GlobalOptions
{
    public static float MusicVolume { get; set; } = 0.05f;
    public static float SfxVolume { get; set; } = 0.05f;
    public static bool Fullscreen { get; set; } = true;
    public static Vector2 SizeScreen { get; set; } = new Vector2(1920, 1080);
}
