using UnityEngine;
using VegetaSystem;

namespace MercuryStudioAssignment
{
    public class SpinMotion
    {
        public enum Phase { Idle, Anticipate, SpinUp, Cruise, Approaching, Impact, Settle, Done }

        private readonly SpinMotionConfig _config;

        public SpinMotion(SpinMotionConfig config)
        {
            _config = config;
        }

        public Phase Current { get; private set; } = Phase.Idle;
        public float Distance { get; private set; }
        public float BounceDip => _config.BounceDip;

        public bool IsActive => Current != Phase.Idle && Current != Phase.Done;
        public bool IsCruising => Current == Phase.Cruise;
        public bool IsStopping => Current == Phase.Approaching || Current == Phase.Impact || Current == Phase.Settle;

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
            Current = Phase.Approaching;
            _phaseTime = 0f;
        }

        public void Tick(float dt)
        {
            _phaseTime += dt;
            switch (Current)
            {
                case Phase.Anticipate: TickAnticipate(); break;
                case Phase.SpinUp: TickSpinUp(); break;
                case Phase.Cruise: Distance += _config.CruiseSpeed * dt; break;
                case Phase.Approaching: TickApproaching(dt); break;
                case Phase.Impact: TickImpact(dt); break;
                case Phase.Settle: TickSettle(); break;
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
            float t = Mathf.Clamp01(_phaseTime / _config.AnticipateDuration);
            Distance = _phaseStartDistance - Easing.OutQuad(t) * _config.AnticipateDistance;
            if (t >= 1f) EnterPhase(Phase.SpinUp);
        }

        private void TickSpinUp()
        {
            float t = Mathf.Clamp01(_phaseTime / _config.SpinUpDuration);
            float covered = _config.CruiseSpeed * _config.SpinUpDuration * Easing.InCubic(t) / 3f;
            Distance = _phaseStartDistance + covered;
            if (t >= 1f) EnterPhase(Phase.Cruise);
        }

        private void TickApproaching(float dt)
        {
            Distance += _config.CruiseSpeed * dt;
            if (Distance >= _targetDistance)
            {
                Distance = _targetDistance;
                EnterPhase(Phase.Impact);
            }
        }

        private void TickImpact(float dt)
        {
            Distance += _config.CruiseSpeed * dt;
            if (Distance >= _targetDistance + _config.BounceDip)
            {
                Distance = _targetDistance + _config.BounceDip;
                EnterPhase(Phase.Settle);
            }
        }

        private void TickSettle()
        {
            float t = Mathf.Clamp01(_phaseTime / _config.BounceDuration);
            Distance = (_targetDistance + _config.BounceDip) - Easing.OutQuad(t) * _config.BounceDip;
            if (t >= 1f)
            {
                Distance = _targetDistance;
                Current = Phase.Done;
            }
        }
    }
}
