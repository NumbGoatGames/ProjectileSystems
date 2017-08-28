using UnityEngine;

namespace NumbGoat.Projectile {
    public static class TrajectoryHelper {
        /// <summary>
        ///     Methods for projectile intercept of moving target.
        ///     Also takes into account velocity of shooter
        ///     (unsure if necessary - should only be needed if shots inherit velocity).
        ///     Needs position and velocity of both source and target objects.
        ///     Also needs shot speed;
        ///     Note not tested yet.
        /// </summary>
        /// <param name="sourcePos">The position of the shooter.</param>
        /// <param name="sourceVel">The velocity of the shooter.</param>
        /// <param name="targetPos">The position of the target.</param>
        /// <param name="targetVel">The velocity of the target.</param>
        /// <param name="shotSpeed">The horizontal speed of the shot.</param>
        /// <returns></returns>
        public static Vector3 InterceptPosition(Vector3 sourcePos, Vector3 sourceVel, Vector3 targetPos,
            Vector3 targetVel, float shotSpeed) {
            Vector3 targetRelPos = targetPos - sourcePos;
            Vector3 targetRelVel = targetVel - sourceVel;

            float t = InterceptTime(targetRelPos, targetRelVel, shotSpeed);

            return targetPos + t * targetRelVel;
        }

        /// <summary>
        ///     Calculates the lead time needed to intercept a moving object with a shot
        ///     Needs the relative position and velocity of the target, and the speed of the shot
        ///     NOTE NOT TESTED YET.
        /// </summary>
        /// <param name="targetRelPos"></param>
        /// <param name="targetRelVel"></param>
        /// <param name="shotSpeed"></param>
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

        /// <summary>
        ///     Calculates the angle needed to land a projectile on a given point.
        ///     Needs the origin and target positions, as well as the speed of the shot.
        ///     Optionally needs gravity value (if different from global Physics gravity amount).
        ///     Currently returns only positive firing angle,
        ///     but negative might be useful for closer targets.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <param name="v"></param>
        /// <param name="negAngle"></param>
        /// <returns></returns>
        public static float? CalculateProjectileAngle(Vector3 origin, Vector3 target, float v,
            bool negAngle = false) {
            float y = origin.y - target.y;
            float xx = target.x - origin.x;
            float xz = target.z - origin.z;
            float x = Mathf.Sqrt(xx * xx + xz * xz);
            float grav = Physics.gravity.y; // can replace this with parameter value if needed

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
    }
}