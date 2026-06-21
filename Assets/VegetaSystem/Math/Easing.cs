using UnityEngine;

namespace VegetaSystem
{
    /// <summary>
    /// Pure-math easing functions. Each takes a normalized time t (0..1) and returns a curved
    /// progress. Use to smooth a value: result = from + (to - from) * Easing.X(t).
    /// "Order" = exponent: order 1 is linear (t), order 2 (Quad) curves gently, order 3 (Cubic)
    /// curves harder. ease-IN = slow start / fast end; ease-OUT = fast start / slow end.
    /// </summary>
    public static class Easing
    {
        /// <summary>
        /// Ease-out, order 2: 1 - (1-t)^2. Fast at the start, eases to a soft stop (t=1 -> 1).
        /// Use for motion approaching a point and settling. Higher order = harder slow-down.
        /// </summary>
        public static float OutQuad(float t)
        {
            t = Mathf.Clamp01(t);
            return 1f - (1f - t) * (1f - t);
        }

        /// <summary>
        /// Ease-in, order 3: t^3. Slow at the start, accelerates toward the end (sharper than
        /// order 2). Use for ramping up from rest, e.g. spin-up. Returns position, not velocity.
        /// </summary>
        public static float InCubic(float t)
        {
            t = Mathf.Clamp01(t);
            return t * t * t;
        }

        /// <summary>
        /// Half sine (sin(t*PI) goes 0 -> 1 -> 0, one hump) decayed by e^(-damping*t).
        /// Models one damped overshoot around 0: swings out then returns. High damping = shallow
        /// and dies out fast; low damping = deeper and lingers. The half sine sets the rhythm
        /// (fast at both ends, near-still at the peak). Multiply by amplitude for a bounce/settle.
        /// </summary>
        public static float DampedHalfSine(float t, float damping)
        {
            t = Mathf.Clamp01(t);
            return Mathf.Exp(-damping * t) * Mathf.Sin(t * Mathf.PI);
        }
    }
}
