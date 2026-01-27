using System;
using System.Numerics;

namespace Application.Dto;

public static class GlobalOptions
{
    public static int MusicVolume { get; set; } = 50;
    public static int SfxVolume { get; set; } = 50;
    public static bool Fullscreen { get; set; } = true;
    public static Vector2 SizeScreen { get; set; } = new Vector2(1920, 1080);

    public static float MusicVolumeFloat => VolumeToFloat(MusicVolume);
    public static float SfxVolumeFloat => VolumeToFloat(SfxVolume);

    private static float VolumeToFloat(int slider)
    {
        float t = Math.Clamp(slider / 100f, 0f, 1f);
        return MathF.Pow(t, 2.2f);
    }
}
