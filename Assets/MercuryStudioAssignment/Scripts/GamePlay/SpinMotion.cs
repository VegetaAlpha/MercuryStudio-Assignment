using System;
using UnityEngine;

namespace MercuryStudioAssignment
{
    [Serializable]
    public class SpinMotion
    {
        public enum Phase { Idle, Anticipate, SpinUp, Cruise, Approaching, Bounce, Done }

        [Header("Anticipate (lùi lấy đà)")]
        [SerializeField] private float _anticipateDistance = 60f;
        [SerializeField] private float _anticipateDuration = 0.12f;

        [Header("Spin up")]
        [SerializeField] private float _cruiseSpeed = 2600f;
        [SerializeField] private float _spinUpDuration = 0.25f;

        [Header("Bounce (dip xuống rồi nảy lên settle)")]
        [SerializeField] private float _bounceDip = 70f;
        [SerializeField] private float _bounceDuration = 0.35f;
        [SerializeField] private float _bounceDamping = 6f;

        public Phase Current { get; private set; } = Phase.Idle;
        public float Distance { get; private set; }

        public bool IsActive => Current != Phase.Idle && Current != Phase.Done;
        public bool IsCruising => Current == Phase.Cruise;
        public bool IsStopping => Current == Phase.Approaching || Current == Phase.Bounce;

        private float _phaseTime;
        private float _phaseStartDistance;
        private float _targetDistance;

        public void ResetTo(float distance)
        {
            Current = Phase.Idle;
            Distance = distance;
            _phaseTime = 0f;
        }

        public void Begin()
        {
            EnterPhase(Phase.Anticipate);
        }

        public void RequestStop(float targetCenterDistance)
        {
            _targetDistance = targetCenterDistance;
            Current = Phase.Approaching; // giữ nguyên tốc độ, không giảm tốc
            _phaseTime = 0f;
        }

        public void Tick(float dt)
        {
            _phaseTime += dt;
            switch (Current)
            {
                case Phase.Anticipate: TickAnticipate(); break;
                case Phase.SpinUp: TickSpinUp(); break;
                case Phase.Cruise: Distance += _cruiseSpeed * dt; break;
                case Phase.Approaching: TickApproaching(dt); break;
                case Phase.Bounce: TickBounce(); break;
            }
        }

        private void EnterPhase(Phase phase)
        {
            Current = phase;
            _phaseTime = 0f;
            _phaseStartDistance = Distance;
        }

        private void TickAnticipate()
        {
            float t = Mathf.Clamp01(_phaseTime / _anticipateDuration);
            float offset = -Mathf.Sin(t * Mathf.PI) * _anticipateDistance; // lùi lên rồi về
            Distance = _phaseStartDistance + offset;
            if (t >= 1f) EnterPhase(Phase.SpinUp);
        }

        private void TickSpinUp()
        {
            float t = Mathf.Clamp01(_phaseTime / _spinUpDuration);
            float covered = _cruiseSpeed * _spinUpDuration * (t * t * t) / 3f; // ∫ cruise·u² du
            Distance = _phaseStartDistance + covered;
            if (t >= 1f) EnterPhase(Phase.Cruise);
        }

        private void TickApproaching(float dt)
        {
            Distance += _cruiseSpeed * dt;
            if (Distance >= _targetDistance)
            {
                Distance = _targetDistance;
                EnterPhase(Phase.Bounce);
            }
        }

        private void TickBounce()
        {
            float t = Mathf.Clamp01(_phaseTime / _bounceDuration);
            float decay = Mathf.Exp(-_bounceDamping * t);
            float offset = _bounceDip * decay * Mathf.Sin(t * Mathf.PI); // dip xuống rồi tắt dần
            Distance = _targetDistance + offset;
            if (t >= 1f)
            {
                Distance = _targetDistance;
                Current = Phase.Done;
            }
        }
    }
}
