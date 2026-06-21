using UnityEngine;

namespace MercuryStudioAssignment
{
    [CreateAssetMenu(fileName = "GameFlowConfig", menuName = "MonsterWheel/GameFlowConfig")]
    public class GameFlowConfig : ScriptableObject
    {
        [Tooltip("Phải cruise ít nhất bấy nhiêu giây (tính từ start) mới cho phép dừng.")]
        public float MinCruiseTime = 1.0f;
        [Tooltip("Sau khi nhấn stop, chạy max thêm bấy nhiêu giây rồi mới giảm tốc dừng.")]
        public float StopDelay = 0.6f;
    }
}
