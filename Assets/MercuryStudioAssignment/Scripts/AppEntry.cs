using UnityEngine;

namespace MercuryStudioAssignment
{
    /// <summary>
    /// Application entry point. Runs once at startup, before any scene loads,
    /// to set the target frame rate to 60 FPS.
    /// </summary>
    public static class AppEntry
    {
        private const int TargetFrameRate = 60;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            // Disable VSync so Application.targetFrameRate is respected.
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = TargetFrameRate;
        }
    }
}
