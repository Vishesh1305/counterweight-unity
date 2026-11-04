using UnityEngine;

// Forces the standalone build into a 1920x1080 window on every launch.
public static class ResolutionEnforcer
{
    private const int TargetWidth = 1920;
    private const int TargetHeight = 1080;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ApplyResolution()
    {
        Screen.SetResolution(TargetWidth, TargetHeight, FullScreenMode.Windowed);
    }
}