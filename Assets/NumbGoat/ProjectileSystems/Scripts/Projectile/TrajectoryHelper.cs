using UnityEngine;

namespace NumbGoat.ProjectileSystems.Scripts.Projectile {
    /// <summary>
    ///     Helper class for trajectory calculations.
    ///     Thanks JamesLeeNZ and other posters on this forum thread!
    ///     https://forum.unity3d.com/threads/projectile-trajectory-accounting-for-gravity-velocity-mass-distance.425560/#post-2750631
    /// </summary>
    public static class TrajectoryHelper {
        /// <summary>
        ///     Determine the first-order intercept using absolute target position.
        /// </summary>
        /// <param name="shooterPosition">Position of shooter</param>
        /// <param name="shooterVelocity">Current velocity of shooter</param>
        /// <param name="shotSpeed">Speed of the shot (projectile)</param>
        /// <param name="targetPosition">Position of target</param>
        /// <param name="targetVelocity">Velocity of target</param>
        /// <returns>First order intercept</returns>
        public static Vector3 FirstOrderIntercept(Vector3 shooterPosition, Vector3 shooterVelocity, float shotSpeed,
            Vector3 targetPosition, Vector3 targetVelocity) {
            Vector3 targetRelativePosition = targetPosition - shooterPosition;
            Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
            float t = FirstOrderInterceptTime(
                shotSpeed,
                targetRelativePosition,
                targetRelativeVelocity
            );
            return targetPosition + t * targetRelativeVelocity;
        }

        /// <summary>
        ///     Determine the first-order intercept using relative target position.
        /// </summary>
        /// <param name="shotSpeed">Speed of the shot (projectile)</param>
        /// <param name="targetRelativePosition">Position of target relative to shooter</param>
        /// <param name="targetRelativeVelocity">Current velocity of target relative to shooter</param>
        /// <returns>First order intercept</returns>
        public static float FirstOrderInterceptTime(float shotSpeed, Vector3 targetRelativePosition,
            Vector3 targetRelativeVelocity) {
            float velocitySquared = targetRelativeVelocity.sqrMagnitude;
            if (velocitySquared < 0.001f) {
                return 0f;
            }

            float a = velocitySquared - shotSpeed * shotSpeed;

            //handle similar velocities
            if (Mathf.Abs(a) < 0.001f) {
                float t = -targetRelativePosition.sqrMagnitude /
                          (2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition));
                return Mathf.Max(t, 0f); //don't shoot back in time
            }

            float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
            float c = targetRelativePosition.sqrMagnitude;
            float determinant = b * b - 4f * a * c;

            if (determinant > 0f) {
                //determinant > 0; two intercept paths (most common)
                float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a),
                    t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
                if (t1 > 0f) {
                    if (t2 > 0f) {
                        return Mathf.Min(t1, t2); //both are positive
                    }
                    return t1; //only t1 is positive
                }
                return Mathf.Max(t2, 0f); //don't shoot back in time
            }
            if (determinant < 0f) {
                //determinant < 0; no intercept path
                return 0f;
            }
            return Mathf.Max(-b / (2f * a), 0f); //don't shoot back in time
        }

        /// <summary>
        ///     Calculates the trajectory for a shot.
        /// </summary>
        /// <param name="targetDistance">Distance of target from shooter</param>
        /// <param name="projectileVelocity">Speed of projectile</param>
        /// <param name="calculatedAngle">Trajectory calculated</param>
        /// <returns>True iff it is possible to calculate the trajectory for this situation.</returns>
        public static bool CalculateTrajectory(float targetDistance, float projectileVelocity,
            out float calculatedAngle) {
            calculatedAngle = 0.5f * (Mathf.Asin(-Physics.gravity.y * targetDistance /
                                                 (projectileVelocity * projectileVelocity)) * Mathf.Rad2Deg);
            if (float.IsNaN(calculatedAngle)) {
                calculatedAngle = 0;
                return false;
            }
            return true;
        }

        
        /// <summary>
        ///     Use the Artillery formula to return an angle to hit the target.
        ///     Based on https://gist.github.com/benloong/4661336
        /// </summary>
        /// <param name="from">Where we are shooting from</param>
        /// <param name="to">What we are aiming at</param>
        /// <param name="v">The velocity of the projectile</param>
        /// <param name="g">Gravity,</param>
        /// <param name="posAngle">Aim up to hit target or aim across?</param>
        /// <returns></returns>
        public static float? CalcAngleOfElevation(Vector3 from, Vector3 to, float v, float g, bool posAngle = false) {
//            float x = Mathf.Abs((to - from).z);
            Vector3 fromNoY = new Vector3(from.x, 0, from.z);
            Vector3 toNoY = new Vector3(to.x, 0, to.z);
            float x = Vector3.Distance(fromNoY, toNoY);
            float y = (to - from).y;
            float v2 = v * v;
            float v4 = v2 * v2;
            float fac = v4 - g * (g * x * x + 2 * y * v2);
            if (fac < 0) {
                return null; // No angle
            }
            float theta = 0;
            if (posAngle) {
                theta = -Mathf.Atan((v2 + Mathf.Sqrt(fac)) / (g * x)) * Mathf.Rad2Deg;
            } else {
                theta = -Mathf.Atan((v2 - Mathf.Sqrt(fac)) / (g * x)) * Mathf.Rad2Deg;
            }
//            while (theta < 0) {
//                theta += 360f;
//            }
            return theta;
        }
    }
}