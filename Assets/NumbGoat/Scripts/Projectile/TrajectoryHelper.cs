using UnityEngine;

namespace NumbGoat.Projectile {
    /// <summary>
    ///     Helper class for trajectory calculations.
    /// </summary>
    public static class TrajectoryHelper {
        /// <summary>
        ///     Calculates the angle needed to land a projectile on a given point.
        ///     Optionally needs gravity value (if different from global Physics gravity amount).
        ///     Currently returns only positive firing angle,
        ///     but negative might be useful for closer targets.
        /// </summary>
        /// <param name="origin">Position from which projectile originates</param>
        /// <param name="target">Position of target</param>
        /// <param name="v">Velocity of projectile</param>
        /// <param name="negAngle">True iff this function should return the negative firing angle.</param>
        /// <returns>Angle at which projectile should be shot from origin</returns>
        public static float? CalculateProjectileAngle(
            Vector3 origin, Vector3 target, float v,
            bool negAngle = false) {
            float y = origin.y - target.y;
            float xx = target.x - origin.x;
            float xz = target.z - origin.z;
            float x = Mathf.Sqrt(xx * xx + xz * xz);
            float grav = Physics.gravity.y; // can replace this with a parameter value if needed

            Debug.Log($"Shot initial parameters YDiff: {y}, XDiff: {x}, Grav: {grav} ");

            float x2 = x * x;
            float v2 = v * v;
            float v4 = v * v * v * v;
            float sqrt = v4 - grav * (grav * x2 + 2 * y * v);
            Debug.Log($"Before sqrt {sqrt}");
            if (sqrt < 0) {
                // No possible firing solution.
                return null;
            }

            sqrt = Mathf.Sqrt(sqrt);
            Debug.Log($"After sqrt {sqrt}");
            // could use either of these, depending on if you need to aim down to hit something
            // NOTE: Positive angle will always work if there is a solution
            float anglePos = Mathf.Atan((v2 + sqrt) / (grav * x));
            float angleNeg = Mathf.Atan((v2 - sqrt) / (grav * x));
            Debug.Log($"Determined angle pos:{anglePos} neg:{angleNeg}");
            if (negAngle) {
                return angleNeg * Mathf.Rad2Deg;
            }
            return anglePos * Mathf.Rad2Deg;
        }

        /// <summary>
        ///     Calculates the time required to intercept a moving object with a projectile.
        ///     TODO NOT YET TESTED
        /// </summary>
        /// <param name="sourcePos">The position of the shooter</param>
        /// <param name="sourceVel">The velocity of the shooter</param>
        /// <param name="targetPos">The position of the target</param>
        /// <param name="targetVel">The velocity of the target</param>
        /// <param name="shotSpeed">The horizontal speed of the shot</param>
        /// <returns>Time required to intercept target</returns>
        public static Vector3 InterceptPosition(
            Vector3 sourcePos, Vector3 sourceVel, Vector3 targetPos,
            Vector3 targetVel, float shotSpeed) {
            Vector3 targetRelPos = targetPos - sourcePos;
            Vector3 targetRelVel = targetVel - sourceVel;

            float t = InterceptTime(targetRelPos, targetRelVel, shotSpeed);

            return targetPos + t * targetRelVel;
        }

        /// <summary>
        ///     Calculates the lead time needed to intercept a moving object with a projectile.
        ///     TODO NOT YET TESTED
        /// </summary>
        /// <param name="targetRelPos">Relative position of target</param>
        /// <param name="targetRelVel">Relative velocity of target</param>
        /// <param name="shotSpeed">Speed at which projectile is moving</param>
        /// <returns></returns>
        public static float InterceptTime(Vector3 targetRelPos, Vector3 targetRelVel, float shotSpeed) {
            float velSquared = targetRelVel.sqrMagnitude;
            if (velSquared < 0.001f) {
                return 0f;
            }

            // Maths here
            float a = velSquared - shotSpeed * shotSpeed;

            // handle similar velocities
            if (Mathf.Abs(a) < 0.001f) {
                float t = -targetRelPos.sqrMagnitude /
                          (2f * Vector3.Dot(targetRelVel, targetRelPos));
                return Mathf.Max(t, 0f); // dont shoot back in time
            }

            float b = 2f * Vector3.Dot(targetRelVel, targetRelPos);
            float c = targetRelPos.sqrMagnitude;
            float determinant = b * b - 4f * a * c;

            if (determinant > 0f) {
                // two intercept paths
                float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a);
                float t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);

                if (t1 > 0f) {
                    if (t2 > 0f) {
                        // both are positive
                        return Mathf.Min(t1, t2);
                    }
                    // only t1 is positive
                    return t1;
                }
                return Mathf.Max(t2, 0f); // dont shoot back in time
            }
            if (determinant < 0f) {
                // no intercept path
                return 0f;
            }
            // determinant == 0, one intercept path
            return Mathf.Max(-b / (2f * a), 0f);
        }
    }
}