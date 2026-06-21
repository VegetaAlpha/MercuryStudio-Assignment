using UnityEngine;

namespace MercuryStudioAssignment
{
    [CreateAssetMenu(fileName = "BorderWheelConfig", menuName = "MonsterWheel/BorderWheelConfig")]
    public class BorderWheelConfig : ScriptableObject
    {
        [Header("Layout — khung chữ nhật, slot 0 ở góc TRÁI-TRÊN")]
        [Tooltip("Số card hàng TRÊN (= dưới), góc nằm trong số này.")]
        public int Cols = 4;
        [Tooltip("Số card cạnh TRÁI (= phải), KHÔNG tính 2 góc.")]
        public int Rows = 5;
        public float FrameWidth = 450f;
        public float FrameHeight = 900f;

        [Header("Sequence")]
        [Tooltip("Monster id của slot đầu (góc trái-trên). Slot sau = (StartId+i)%count.")]
        public int StartId = 0;

        [Header("Highlight — enable bật thẳng, disable lerp tắt dần (vệt đuôi)")]
        public Color DimColor = new Color(0.18f, 0.18f, 0.2f, 1f);
        public Color LitColor = Color.white;
        [Tooltip("Tốc độ lerp tắt sáng về dim (đơn vị/giây). Cao = tắt nhanh.")]
        public float DimLerpSpeed = 6f;

        [Header("Spin")]
        [Tooltip("Tốc độ quay tối đa (slot/giây).")]
        public float SpinSpeed = 14f;
        [Tooltip("Số con (slot) đi qua trong lúc tăng tốc → max. Video ~9-10 con mới đạt max.")]
        public float SpinUpSlots = 10f;
        [Range(0.02f, 1f)]
        [Tooltip("Sàn tốc độ lúc đầu = phần này của max (nhúc nhích ngay, tránh kẹt). Đường ease-in t².")]
        public float SpinUpStartFraction = 0.08f;
        [Tooltip("Số object CUỐI được phanh (giảm tốc đều, mỗi con chậm dần thấy rõ). Vd 6.")]
        public int DecelSlots = 6;
        [Tooltip("Thời gian (giây) phanh qua DecelSlots con cuối. Lớn = mỗi con chậm rõ hơn. Vd 0.8.")]
        public float DecelDuration = 0.8f;
    }
}
