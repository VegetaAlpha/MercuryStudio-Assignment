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

        // TEST MODE: chỉ kiểm tra cuộn giữa. Space bật/tắt cuộn đều, chưa dùng anim/stop/wheel.
        [SerializeField] private float _testScrollSpeed = 2000f;
        private bool _testScrolling;
        private float _testDistance;

        private void Start()
        {
            _scroller.Initialize();
            _motion.ResetTo(0f);
        }

        private void Update()
        {
            if (GetSpinInput()) _testScrolling = !_testScrolling;

            if (_testScrolling)
                _testDistance += _testScrollSpeed * Time.deltaTime;

            _scroller.Layout(_testDistance);
        }

        private bool GetSpinInput()
        {
            return Input.GetKeyDown(KeyCode.Space) || _uiPressed;
        }

        public void OnSpinButton() => _uiPressed = true;

        // FULL MODE (bật lại khi làm anim start/stop + wheel):
        //   Update: GetSpinInput -> OnSpinPressed; _motion.Tick; _scroller.Layout(_motion.Distance);
        //           Stopping + Phase.Done -> Idle; reset _uiPressed.
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
