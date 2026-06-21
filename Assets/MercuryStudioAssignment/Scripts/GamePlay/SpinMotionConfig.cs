using UnityEngine;

namespace MercuryStudioAssignment
{
    [CreateAssetMenu(fileName = "SpinMotionConfig", menuName = "MonsterWheel/SpinMotionConfig")]
    public class SpinMotionConfig : ScriptableObject
    {
        [Header("Anticipate (lùi lấy đà)")]
        public float AnticipateDistance = 80f;
        public float AnticipateDuration = 0.2f;

        [Header("Spin up")]
        public float CruiseSpeed = 1250f;
        public float SpinUpDuration = 0.25f;

        [Header("Bounce — nửa xuống đập theo cruise, nửa lên ease-out về tâm")]
        public float BounceDip = 100f;
        public float BounceDuration = 0.5f;
    }
}
