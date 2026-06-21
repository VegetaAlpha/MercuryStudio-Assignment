using UnityEngine;

namespace MercuryStudioAssignment
{
    [CreateAssetMenu(fileName = "SpinMotionConfig", menuName = "MonsterWheel/SpinMotionConfig")]
    public class SpinMotionConfig : ScriptableObject
    {
        [Header("Layout center scroll")]
        [Tooltip("Khoảng cách tâm-đến-tâm giữa 2 cell (px).")]
        public float Step = 240f;
        [Tooltip("Số cell đệm thêm ngoài viewport mỗi phía.")]
        public int Buffer = 1;

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
