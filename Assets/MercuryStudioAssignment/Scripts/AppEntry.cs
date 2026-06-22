using UnityEngine;

namespace MercuryStudioAssignment
{
    /// <summary>
    /// Application entry point. Runs once at startup, before any scene loads.
    /// Disables logging for performance, then sets the frame rate per platform:
    ///  - PC (Standalone): left at project default (VSync on) so it follows the
    ///    player's monitor refresh rate. The game is very light, so any rate is fine.
    ///  - Mobile (Android/iOS): capped at 60 with VSync off, to save battery/heat
    ///    (matches the assignment target of smooth 60fps on mobile).
    /// </summary>
    public static class AppEntry
    {
        private const int MobileFrameRate = 60;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            Debug.unityLogger.logEnabled = false;

#if UNITY_STANDALONE && !UNITY_EDITOR
            // PC: keep VSync (project default) -> follows monitor refresh rate.
#elif UNITY_ANDROID || UNITY_IOS
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = MobileFrameRate;
#endif
        }
    }
}
