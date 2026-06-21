using UnityEngine;

namespace MercuryStudioAssignment
{
    public class MonsterWheelController : MonoBehaviour
    {
        [SerializeField] private MonsterCatalog _catalog;
        [SerializeField] private CenterScroller _scroller;
        [SerializeField] private BorderWheel _wheel;

        private bool _uiPressed;

        private void Start()
        {
            _scroller.Initialize();
            _wheel.Initialize();
        }

        private void Update()
        {
            if (GetSpinInput()) OnSpinPressed();

            _scroller.Tick(Time.deltaTime);
            _wheel.Tick(Time.deltaTime);

            _uiPressed = false;
        }

        private bool GetSpinInput()
        {
            return Input.GetKeyDown(KeyCode.Space) || _uiPressed;
        }

        public void OnSpinButton() => _uiPressed = true;

        private void OnSpinPressed()
        {
            if (!_scroller.IsSpinning) StartSpin();
            else if (!_scroller.IsStopping) BeginStop();
        }

        private void StartSpin()
        {
            _scroller.BeginSpin();
            _wheel.StartSpin();
        }

        private void BeginStop()
        {
            int targetId = _catalog.RandomId();
            _scroller.Stop(targetId);
            _wheel.StopOn(targetId);
        }
    }
}
