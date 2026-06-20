using UnityEngine;
using VegetaSystem;

namespace MercuryStudioAssignment
{
    public class CenterScroller : MonoBehaviour
    {
        [SerializeField] private MonsterCatalog _catalog;
        [SerializeField] private RectTransform _viewport;
        [SerializeField] private RectTransform _content;

        [Tooltip("Khoảng cách tâm-đến-tâm giữa 2 cell (px).")]
        [SerializeField] private float _step = 240f;

        [Tooltip("Số cell đệm thêm ngoài viewport mỗi phía.")]
        [SerializeField] private int _buffer = 1;

        private MonsterCell[] _cells;
        private float _baseTopY;
        private int _headIdx;
        private int _topRow;
        private float _killY;

        private int _forcedRow = int.MaxValue;
        private int _forcedMonsterId = -1;

        public float Step => _step;
        public bool Initialized { get; private set; }

        public void Initialize()
        {
            if (Initialized) return;

            float vh = _viewport.rect.height;
            int visible = Mathf.CeilToInt(vh / _step) + 1;
            int count = visible + _buffer * 2;

            _baseTopY = vh * 0.5f + _buffer * _step;
            _killY = -(vh * 0.5f) - _step;

            _cells = new MonsterCell[count];
            _headIdx = 0;
            _topRow = 0;
            for (int i = 0; i < count; i++)
            {
                var cell = PoolSystem.Instance.GetObj<MonsterCell>();
                cell.Rect.SetParent(_content, false);
                _cells[i] = cell;
                BindRow(cell, _topRow + i);
            }

            Layout(0f);
            Initialized = true;
        }

        public void Layout(float distance)
        {
            int n = _cells.Length;

            int bottomRow = _topRow + (n - 1);
            while (RowY(bottomRow, distance) < _killY)
            {
                int tailIdx = (_headIdx + n - 1) % n; // physical slot of the bottom cell
                _headIdx = tailIdx;                   // it becomes the new top cell
                _topRow--;
                BindRow(_cells[tailIdx], _topRow);
                bottomRow = _topRow + (n - 1);
            }

            for (int offset = 0; offset < n; offset++)
            {
                int physical = (_headIdx + offset) % n;
                int row = _topRow + offset;
                var rect = _cells[physical].Rect;
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, RowY(row, distance));
            }
        }

        // distance↑ ⇒ y giảm ⇒ cell đi xuống. Row nhỏ ở trên, row lớn ở dưới.
        private float RowY(int row, float distance) => _baseTopY - row * _step - distance;

        private void BindRow(MonsterCell cell, int row)
        {
            int id = (row == _forcedRow) ? _forcedMonsterId : _catalog.RandomId();
            cell.Bind(id, _catalog.GetSprite(id));
        }

        public float PlanStop(float currentDistance, int monsterId, int minRowsAhead = 3)
        {
            int centerRow = Mathf.CeilToInt((_baseTopY + currentDistance) / _step);
            int targetRow = centerRow + Mathf.Max(1, minRowsAhead);

            _forcedRow = targetRow;
            _forcedMonsterId = monsterId;

            int n = _cells.Length;
            int offset = targetRow - _topRow;
            if (offset >= 0 && offset < n)
            {
                int physical = (_headIdx + offset) % n;
                _cells[physical].Bind(monsterId, _catalog.GetSprite(monsterId));
            }

            return targetRow * _step - _baseTopY;
        }
    }
}
