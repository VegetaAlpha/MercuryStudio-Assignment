using UnityEngine;

namespace MercuryStudioAssignment
{
    public class MonsterWheelGame : MonoBehaviour
    {
        [SerializeField] private MonsterCatalog _catalog;
        [SerializeField] private CenterScroller _scroller;
        [SerializeField] private SpinMotion _motion = new SpinMotion();

        private enum State { Idle, Spinning, Stopping }
        private State _state = State.Idle;

        private bool _uiPressed;

        private void Start()
        {
            _scroller.Initialize(_motion.BounceDip);
            _motion.ResetTo(0f);
        }

        private void Update()
        {
            if (GetSpinInput()) OnSpinPressed();

            _motion.Tick(Time.deltaTime);
            _scroller.Layout(_motion.Distance);

            if (_state == State.Stopping && _motion.Current == SpinMotion.Phase.Done)
                _state = State.Idle;

            _uiPressed = false;
        }

        private bool GetSpinInput()
        {
            return Input.GetKeyDown(KeyCode.Space) || _uiPressed;
        }

        public void OnSpinButton() => _uiPressed = true;

        private void OnSpinPressed()
        {
            switch (_state)
            {
                case State.Idle: StartSpin(); break;
                case State.Spinning: BeginStop(); break;
                case State.Stopping: break;
            }
        }

        private void StartSpin()
        {
            _state = State.Spinning;
            _motion.Begin();
        }

        private void BeginStop()
        {
            _state = State.Stopping;
            int targetId = _catalog.RandomId();
            float targetDistance = _scroller.PlanStop(_motion.Distance, targetId);
            _motion.RequestStop(targetDistance);
        }
    }
}
