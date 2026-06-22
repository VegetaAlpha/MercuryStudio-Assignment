using UnityEngine;
using UnityEngine.UI;

namespace MercuryStudioAssignment
{
    public class MonsterWheelController : MonoBehaviour
    {
        [SerializeField] Button _spaceBtn;
        [SerializeField] private MonsterCatalog _catalog;
        [SerializeField] private CenterScroller _scroller;
        [SerializeField] private BorderWheel _wheel;
        [SerializeField] private GameFlowConfig _flowConfig;

        private bool _uiPressed;
        private float _spinTime;
        private bool _stopPending;
        private float _stopAt;

        private void Awake()
        {
            _spaceBtn.onClick.AddListener(OnSpinButton);
        }

        private void OnDestroy()
        {
            _spaceBtn.onClick.RemoveAllListeners();
        }

        private void Start()
        {
            _scroller.Initialize(_catalog);
            _wheel.Initialize(_catalog);
        }

        private void Update()
        {
            if (GetSpinInput()) OnSpinPressed();

            if (_scroller.IsSpinning) _spinTime += Time.deltaTime;

            if (_stopPending && _spinTime >= _stopAt)
            {
                _stopPending = false;
                DoStop();
            }

            _scroller.Tick(Time.deltaTime);
            _wheel.Tick(Time.deltaTime);

            _uiPressed = false;
        }

        private bool GetSpinInput()
        {
            return _uiPressed; // Input.GetKeyDown(KeyCode.Space) || 
        }

        public void OnSpinButton() => _uiPressed = true;

        private void OnSpinPressed()
        {
            if (!_scroller.IsSpinning) StartSpin();
            else if (!_scroller.IsStopping && !_stopPending) RequestStop();
        }

        private void StartSpin()
        {
            _spinTime = 0f;
            _stopPending = false;
            _scroller.BeginSpin();
            _wheel.StartSpin();
        }

        private void RequestStop()
        {
            _stopAt = Mathf.Max(_spinTime + _flowConfig.StopDelay, _flowConfig.MinCruiseTime);
            _stopPending = true;
        }

        private void DoStop()
        {
            int targetId = _catalog.RandomId();
            _scroller.Stop(targetId);
            _wheel.StopOn(targetId);
        }
    }
}
