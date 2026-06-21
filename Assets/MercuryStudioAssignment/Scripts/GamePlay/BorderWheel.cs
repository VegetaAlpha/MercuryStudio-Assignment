using UnityEngine;
using UnityEngine.UI;
using VegetaSystem;

namespace MercuryStudioAssignment
{
    public class BorderWheel : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private Image _slotPrefab;
        [SerializeField] private BorderWheelConfig _config;

        private MonsterCatalog _catalog;

        private enum State { Idle, SpinUp, Cruise, Decel }
        private State _state = State.Idle;

        private Image[] _slots;
        private int[] _slotIds;
        private int _slotCount;

        private float _cursor;          // lit-cursor position (cumulative slot, not wrapped)
        private float _phaseTime;
        private float _spinUpFrom;
        private float _decelFrom;
        private float _stopTarget;
        private int _litSlot = -1;

        public bool Initialized { get; private set; }
        public bool IsSpinning => _state != State.Idle;
        public bool IsStopping => _state == State.Decel;

        public void Initialize(MonsterCatalog catalog)
        {
            if (Initialized) return;

            _catalog = catalog;
            _slotCount = 2 * _config.Cols + 2 * _config.Rows;
            _slots = new Image[_slotCount];
            _slotIds = new int[_slotCount];

            int count = _catalog.Count;
            for (int i = 0; i < _slotCount; i++)
            {
                Image slot = Instantiate(_slotPrefab, _root);
                slot.rectTransform.anchoredPosition = PerimeterPos(i);

                int id = ((_config.StartId + i) % count + count) % count;
                slot.sprite = _catalog.GetSprite(id);
                slot.color = _config.DimColor;

                _slots[i] = slot;
                _slotIds[i] = id;
            }

            // start the cursor at a random slot so the first spin isn't always top-left.
            _cursor = Random.Range(0, _slotCount);

            Initialized = true;
        }

        // slot 0 = top-left, walk CW: top → right → bottom → left. Corners belong to the
        // horizontal rows; vertical sides hold Rows cards in the MIDDLE (corners excluded).
        private Vector2 PerimeterPos(int i)
        {
            int cols = _config.Cols;
            int rows = _config.Rows;
            float halfW = _config.FrameWidth * 0.5f;
            float halfH = _config.FrameHeight * 0.5f;
            float stepX = cols > 1 ? _config.FrameWidth / (cols - 1) : 0f;
            float stepY = _config.FrameHeight / (rows + 1);

            if (i < cols)
                return new Vector2(-halfW + stepX * i, halfH);

            i -= cols;
            if (i < rows)
                return new Vector2(halfW, halfH - stepY * (i + 1));

            i -= rows;
            if (i < cols)
                return new Vector2(halfW - stepX * i, -halfH);

            i -= cols;
            return new Vector2(-halfW, -halfH + stepY * (i + 1));
        }

        public void StartSpin()
        {
            _state = State.SpinUp;
            _spinUpFrom = _cursor;
            SetLitInstant((int)Mathf.Repeat(Mathf.Floor(_cursor), _slotCount));
        }

        public void StopOn(int monsterId)
        {
            _stopTarget = FindStopAhead(monsterId, _config.DecelSlots);
            _decelFrom = _cursor;
            _phaseTime = 0f;
            _state = State.Decel;
        }

        public void Tick(float dt)
        {
            switch (_state)
            {
                case State.SpinUp:   TickSpinUp(dt); break;
                case State.Cruise:   _cursor += _config.SpinSpeed * dt; break;
                case State.Decel:    TickDecel(dt); break;
            }

            UpdateHighlight(dt);
        }

        private void TickSpinUp(float dt)
        {
            // progress measured by DISTANCE travelled (slots), not time.
            float traveled = _cursor - _spinUpFrom;
            float t = Mathf.Clamp01(traveled / _config.SpinUpSlots);
            // StartFraction is a floor so it moves immediately (avoids stall when traveled=0).
            float ease = Mathf.Max(_config.SpinUpStartFraction, t * t);
            _cursor += _config.SpinSpeed * ease * dt;
            if (t >= 1f) _state = State.Cruise;
        }

        // Linear-velocity decel (= OutQuad on position): each of the last DecelSlots cards slows
        // evenly over DecelDuration seconds, so all of them read as visibly slowing down.
        private void TickDecel(float dt)
        {
            _phaseTime += dt;
            float dist = _stopTarget - _decelFrom;
            float t = _config.DecelDuration > 0f ? Mathf.Clamp01(_phaseTime / _config.DecelDuration) : 1f;
            _cursor = _decelFrom + Easing.OutQuad(t) * dist;
            if (t >= 1f)
            {
                _cursor = _stopTarget;
                SetLitInstant((int)Mathf.Repeat(_stopTarget, _slotCount));
                _state = State.Idle;
            }
        }

        // Cumulative integer position of a slot holding monsterId, ahead of the cursor by >= minAhead.
        // Integer so Floor(result)%slotCount == that slot (stops exactly on it).
        private int FindStopAhead(int monsterId, float minAhead)
        {
            int best = int.MaxValue;
            for (int slot = 0; slot < _slotCount; slot++)
            {
                if (_slotIds[slot] != monsterId) continue;
                int cand = Mathf.CeilToInt(_cursor) - ((Mathf.CeilToInt(_cursor) % _slotCount + _slotCount) % _slotCount) + slot;
                while (cand - _cursor < minAhead) cand += _slotCount;
                if (cand < best) best = cand;
            }
            return best;
        }

        // Enable = instant; disable = lerp toward dim (creates a fading trail behind the cursor).
        private void UpdateHighlight(float dt)
        {
            if (IsSpinning)
                SetLitInstant((int)Mathf.Repeat(Mathf.Floor(_cursor), _slotCount));

            float k = 1f - Mathf.Exp(-_config.DimLerpSpeed * dt);
            for (int i = 0; i < _slotCount; i++)
            {
                if (i == _litSlot) continue;
                _slots[i].color = Color.Lerp(_slots[i].color, _config.DimColor, k);
            }
        }

        private void SetLitInstant(int slot)
        {
            if (slot == _litSlot) return;
            _slots[slot].color = _config.LitColor;
            _litSlot = slot;
        }
    }
}
