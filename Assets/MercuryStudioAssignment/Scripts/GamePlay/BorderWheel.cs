using UnityEngine;
using UnityEngine.UI;

namespace MercuryStudioAssignment
{
    public class BorderWheel : MonoBehaviour
    {
        [SerializeField] private MonsterCatalog _catalog;
        [SerializeField] private RectTransform _root;
        [SerializeField] private Image _slotPrefab;

        [Header("Layout")]
        [SerializeField] private int _slotCount = 16;
        [SerializeField] private float _radiusX = 170f;
        [SerializeField] private float _radiusY = 320f;

        [Header("Highlight")]
        [SerializeField] private Color _dimColor = new Color(0.18f, 0.18f, 0.2f, 1f);
        [SerializeField] private Color _litColor = Color.white;

        [Header("Spin")]
        [SerializeField] private float _spinSpeed = 9f;
        [SerializeField] private float _stopSpeed = 6f;

        private enum State { Idle, Spinning, Stopping }
        private State _state = State.Idle;

        private Image[] _slots;
        private int[] _slotIds;
        private float _cursor;
        private int _targetSlot;
        private int _litSlot = -1;

        public bool Initialized { get; private set; }
        public bool IsStopping => _state == State.Stopping;

        public void Initialize()
        {
            if (Initialized) return;

            _slots = new Image[_slotCount];
            _slotIds = new int[_slotCount];

            for (int i = 0; i < _slotCount; i++)
            {
                Image slot = Instantiate(_slotPrefab, _root);
                float angle = (i / (float)_slotCount) * Mathf.PI * 2f - Mathf.PI * 0.5f;
                slot.rectTransform.anchoredPosition =
                    new Vector2(Mathf.Cos(angle) * _radiusX, Mathf.Sin(angle) * _radiusY);

                int id = _catalog.RandomId();
                slot.sprite = _catalog.GetSprite(id);
                slot.color = _dimColor;

                _slots[i] = slot;
                _slotIds[i] = id;
            }

            Initialized = true;
        }

        public void StartSpin()
        {
            _state = State.Spinning;
            SetLit(-1);
        }

        public void StopOn(int monsterId)
        {
            _targetSlot = FindSlotAhead(monsterId);
            _state = State.Stopping;
        }

        public void Tick(float dt)
        {
            switch (_state)
            {
                case State.Spinning:
                    _cursor = Mathf.Repeat(_cursor + _spinSpeed * dt, _slotCount);
                    SetLit(Mathf.FloorToInt(_cursor));
                    break;

                case State.Stopping:
                    _cursor = Mathf.MoveTowards(_cursor, _targetSlot, _stopSpeed * dt);
                    if (Mathf.Approximately(_cursor, _targetSlot))
                    {
                        int slot = _targetSlot % _slotCount;
                        _cursor = slot;
                        _targetSlot = slot;
                        SetLit(slot);
                        _state = State.Idle;
                    }
                    else
                    {
                        SetLit(Mathf.FloorToInt(_cursor) % _slotCount);
                    }
                    break;
            }
        }

        private int FindSlotAhead(int monsterId)
        {
            int from = Mathf.FloorToInt(_cursor);
            for (int step = 1; step <= _slotCount; step++)
            {
                int i = (from + step) % _slotCount;
                if (_slotIds[i] == monsterId) return from + step;
            }

            // Không slot nào trùng: ép một slot phía trước mang monster đó để vẫn dừng đúng.
            int forced = from + _slotCount / 2;
            int slot = forced % _slotCount;
            _slotIds[slot] = monsterId;
            _slots[slot].sprite = _catalog.GetSprite(monsterId);
            return forced;
        }

        private void SetLit(int slot)
        {
            if (slot == _litSlot) return;
            if (_litSlot >= 0) _slots[_litSlot].color = _dimColor;
            if (slot >= 0) _slots[slot].color = _litColor;
            _litSlot = slot;
        }
    }
}
