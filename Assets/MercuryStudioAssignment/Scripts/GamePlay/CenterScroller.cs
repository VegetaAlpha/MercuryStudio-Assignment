using UnityEngine;
using UnityEngine.UI;
using VegetaSystem;

namespace MercuryStudioAssignment
{
    public class CenterScroller : MonoBehaviour
    {
        [SerializeField] private RectTransform _viewport;
        [SerializeField] private RectTransform _content;

        [SerializeField] private SpinMotionConfig _motionConfig;

        private MonsterCatalog _catalog;
        private SpinMotion _motion;
        private float _step;
        private int _buffer;

        private MonsterCell[] _cells;
        private float _baseTopY;
        private int _headIdx;
        private int _topRow;
        private float _killY;

        private int _forcedRow = int.MaxValue;
        private int _forcedMonsterId = -1;

        public float Step => _step;
        public bool Initialized { get; private set; }

        public bool IsSpinning => _motion.IsActive;
        public bool IsStopping => _motion.IsStopping;

        public void Initialize(MonsterCatalog catalog)
        {
            if (Initialized) return;

            _catalog = catalog;
            _motion = new SpinMotion(_motionConfig);
            _step = _motionConfig.Step;
            _buffer = _motionConfig.Buffer;

            float vh = _viewport.rect.height;
            int rowsAboveCenter = Mathf.CeilToInt((vh * 0.5f) / _step);
            int visible = rowsAboveCenter * 2 + 1;
            int bounceBuffer = Mathf.CeilToInt(_motion.BounceDip / _step) + 1;
            int eff = Mathf.Max(_buffer, bounceBuffer);
            int count = visible + eff * 2;

            _baseTopY = (rowsAboveCenter + eff) * _step;
            _killY = -(vh * 0.5f) - eff * _step;

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

            _motion.ResetTo(0f);
            Layout(0f);
            Initialized = true;
        }

        public void BeginSpin() => _motion.Begin();

        public void Stop(int monsterId)
        {
            float targetDistance = PlanStop(_motion.Distance, monsterId);
            _motion.RequestStop(targetDistance);
        }

        public void Tick(float dt)
        {
            _motion.Tick(dt);
            Layout(_motion.Distance);
        }

        private void Layout(float distance)
        {
            int n = _cells.Length;

            int bottomRow = _topRow + (n - 1);
            while (RowY(bottomRow, distance) < _killY)
            {
                int tailIdx = (_headIdx + n - 1) % n; // bottom cell becomes new top (O(1) recycle)
                _headIdx = tailIdx;
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

        // distance up => y down => cell moves down. Smaller row = higher. (sign-critical)
        private float RowY(int row, float distance) => _baseTopY - row * _step - distance;

        private void BindRow(MonsterCell cell, int row)
        {
            int id = (row == _forcedRow) ? _forcedMonsterId : _catalog.RandomId();
            cell.Bind(id, _catalog.GetSprite(id));
        }

        private float PlanStop(float currentDistance, int monsterId, int minRowsAhead = 3)
        {
            int centerRow = Mathf.FloorToInt((_baseTopY - currentDistance) / _step);
            int targetRow = centerRow - Mathf.Max(1, minRowsAhead);

            _forcedRow = targetRow;
            _forcedMonsterId = monsterId;

            int n = _cells.Length;
            int offset = targetRow - _topRow;
            if (offset >= 0 && offset < n)
            {
                int physical = (_headIdx + offset) % n;
                _cells[physical].Bind(monsterId, _catalog.GetSprite(monsterId));
            }

            return _baseTopY - targetRow * _step;
        }
    }
}
