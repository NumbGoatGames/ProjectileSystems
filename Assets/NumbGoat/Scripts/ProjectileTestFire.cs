using System.Collections;
using NumbGoat.ProjectileSystems.Scripts;
using NumbGoat.ProjectileSystems.Scripts.Projectile;
using UnityEngine;

namespace NumbGoat {
    /// <summary>
    ///     Controller class for testing projectile firing.
    /// </summary>
    public class ProjectileTestFire : MonoBehaviour {
        private bool coRoutineRunning;
        public float Inaccuracy;
        public BaseProjectile Projectile;
        public bool Running = true;
        public float ShotDelaySeconds = 0.15f;
        public float ShotSpeed = 1f;
        private int targetCounter;
        public GameObject[] Targets;
        public bool UseAngleOfReach = true;
        public bool UseNegAngle;

        public void Awake() {
            // Projectiles should not collide with each other for this example.
            Physics.IgnoreLayerCollision(8, 8);
        }

        public void Start() {
            if (this.Projectile == null) {
                Debug.LogError(message: "ProjectileTestFire does not have a projectile.");
                Destroy(this.gameObject.transform.root.gameObject);
                return;
            }

            if (this.Targets.Length == 0) {
                Debug.LogError(message: "Test fire does not have a target.");
                Destroy(this.gameObject.transform.root.gameObject);
            }
        }

        public IEnumerator DoFiring() {
            this.coRoutineRunning = true;
            yield return new WaitForSecondsRealtime(0.2f);
            while (this.Running) {
                if (this.UseAngleOfReach) {
                    this.FireProjectileReach();
                } else {
                    this.FireProjectileArtillery();
                }
                if (++this.targetCounter >= this.Targets.Length) {
                    // Start from the beginning of the list again.
                    this.targetCounter = 0;
                }
                yield return new WaitForSecondsRealtime(this.ShotDelaySeconds);
            }
            this.coRoutineRunning = false;
        }

        private void FireProjectileReach() {
            GameObject targetGameObject = this.Targets[this.targetCounter];
            Transform targetTransform = targetGameObject.transform;

            Vector3 targetVelocity = this.FindTargetVelocity(targetGameObject);
            Vector3 targetCenter = this.FindTargetCenter(targetTransform, targetVelocity);

            // fire at TargetCenter
            BaseProjectile toFire = Instantiate(this.Projectile);
            toFire.Target = targetGameObject; // Set the intended target of the projectile.
            toFire.gameObject.SetActive(true); // Active the projectile (not needed if the prefab is already active).
            toFire.transform.position = this.transform.position; // Set the position to the position of the shooter.
            toFire.transform.LookAt(targetCenter); // Easiest way to get the projectile facing the target.

            // Set projectile in motion, could use:
//            toFire.Rigidbody.AddRelativeForce(0, 0, this.ShotSpeed, ForceMode.Impulse);
            // or if we wish to not have to consider mass.
            toFire.Rigidbody.velocity = toFire.gameObject.transform.forward * this.ShotSpeed;
        }

        private void FireProjectileArtillery() {
            GameObject targetGameObject = this.Targets[this.targetCounter];

            Vector3 firingPositing = this.transform.position;
            Vector3 targetVelocity = this.FindTargetVelocity(targetGameObject);
            Vector3 firstOrderIntercept = TrajectoryHelper.FirstOrderIntercept(
                this.gameObject.transform.position, Vector3.zero,
                this.ShotSpeed, targetGameObject.transform.position, targetVelocity);

            float? trajectoryAngle = TrajectoryHelper.CalcAngleOfElevation(firingPositing, firstOrderIntercept,
                this.ShotSpeed, Physics.gravity.magnitude);

            if (trajectoryAngle == null) {
                Debug.LogError(message: "No trajectory to target.");
                return;
            }
            BaseProjectile toFire = Instantiate(this.Projectile); // TODO: Use projectile pool.
            toFire.Target = targetGameObject; // Set the intended target of the projectile.
            toFire.gameObject.SetActive(true); // Active the projectile (not needed if the prefab is already active).
            toFire.transform.position = firingPositing; // Set the position to the position of the shooter.
            toFire.transform.LookAt(firstOrderIntercept); // Easiest way to get the projectile facing the target.
            toFire.transform.rotation = Quaternion.Euler(trajectoryAngle.Value, toFire.transform.eulerAngles.y,
                toFire.transform.eulerAngles.z); // Look up by the calculated angle

            toFire.Rigidbody.velocity =
                toFire.gameObject.transform.forward * this.ShotSpeed; // Fire forward at set speed


        }

        /// <summary>
        ///     Finds point of target to aim for.
        /// </summary>
        /// <param name="targetTransform">Transform of the target object</param>
        /// <param name="targetVelocity">Current velocity of the target</param>
        /// <returns>Position of target to aim for.</returns>
        private Vector3 FindTargetCenter(Transform targetTransform, Vector3 targetVelocity) {
            float distance = Vector3.Distance(this.transform.position, targetTransform.position);
            float trajectoryAngle;
            Vector3 targetCenter = TrajectoryHelper.FirstOrderIntercept(
                this.transform.position, Vector3.zero,
                this.ShotSpeed, targetTransform.position, targetVelocity);

            if (TrajectoryHelper.CalculateTrajectory(distance, this.ShotSpeed, out trajectoryAngle)) {
                float trajectoryHeight = Mathf.Tan(trajectoryAngle * Mathf.Deg2Rad) * distance;
                targetCenter.y += trajectoryHeight;
            }

            targetCenter = targetCenter + this.GetRandomnessOfShot();
            return targetCenter;
        }

        /// <summary>
        ///     Find the velocity of a target.
        /// </summary>
        /// <param name="targetGameObject">Target object</param>
        /// <returns>Current velocity of targetGameObject.</returns>
        private Vector3 FindTargetVelocity(GameObject targetGameObject) {
            Vector3 targetVelocity = Vector3.zero;
            // Checks for finding velocity,
            IMoving targetGameObjectMoving = targetGameObject.GetComponent<IMoving>();
            // in a real game you should know which of these you have so don't get them just use them.
            if (targetGameObjectMoving != null) {
                // Target implements IMoving, use that as the velocity.
                targetVelocity = targetGameObjectMoving.Velocity;
            } else {
                Rigidbody rb = targetGameObject.GetComponent<Rigidbody>();
                if (rb != null) {
                    // Target has a rigidbody.
                    targetVelocity = rb.velocity;
                }
            }
            return targetVelocity;
        }

        public void Update() {
            if (!this.coRoutineRunning && this.Running) {
                this.StartCoroutine(this.DoFiring());
                this.coRoutineRunning = true;
            }
        }

        private Vector3 GetRandomnessOfShot() {
            return Random.insideUnitSphere * this.Inaccuracy;
        }
    }
}