using System.Collections;
using UnityEngine;

namespace NumbGoat.Projectile {
    /// <summary>
    ///     Controller class for testing projectile firing.
    /// </summary>
    public class ProjectileTestFire : MonoBehaviour {
        public float Inaccuracy;
        public BaseProjectile Projectile;
        public bool Running = true;
        public float ShotSpeed = 1f;
        private int targetCounter;
        public GameObject[] Targets;
        public bool UseNegAngle;

        public void Awake() {
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
                return;
            }
            this.StartCoroutine(this.DoFiring());
        }

        public IEnumerator DoFiring() {
            yield return new WaitForSecondsRealtime(0.5f);
            while (this.Running) {
                this.FireProjectile();
                yield return new WaitForSecondsRealtime(0.25f);
            }
        }

        private void FireProjectile() {
            #region ver1

            //            GameObject targetGameObject = this.Targets[this.targetCounter];
            //            //TODO Projectile pool(?)
            //            Vector3 targetPosition = TrajectoryHelper.InterceptPosition(this.transform.position, Vector3.zero,
            //                targetGameObject.transform.position, Vector3.zero, this.ShotSpeed);
            //            targetPosition += this.GetRandomnessOfShot();
            //            float? angle = TrajectoryHelper.CalculateProjectileAngle(this.gameObject.transform.position,
            //                targetPosition, this.ShotSpeed, this.UseNegAngle);
            //            if (angle == null) {
            //                // if the angle couldn't be calculated, it probably means the target is too far away
            //                // ie. it is impossible to hit the target with the current distance, projectile velocity and gravity
            //                Debug.LogWarning(message: "Could not find angle to hit target.");
            //                return;
            //            }
            //            float notNullAngle = angle.Value;
            //            Debug.Log($"Got firing angle of {notNullAngle}");
            //            BaseProjectile toFire = Instantiate(this.Projectile);
            //            toFire.Target = targetGameObject; // Set the intended target of the projectile.
            //            toFire.gameObject.SetActive(true); // Active the projectile (not needed if the prefab is already active).
            //            toFire.transform.position = this.transform.position; // Set the position to the position of the shooter.
            //            toFire.transform.LookAt(targetPosition); // Easiest way to get the projectile facing the target.
            //            toFire.transform.rotation = Quaternion.Euler(notNullAngle, toFire.transform.eulerAngles.y,
            //                toFire.transform.eulerAngles.z); // Look up at the correct angle.
            //            toFire.Rigidbody.velocity = toFire.transform.forward * this.ShotSpeed; // Set the projectiles velocity.
            //
            //            if (++this.targetCounter >= this.Targets.Length) {
            //                // Start from the beginning of the list again.
            //                this.targetCounter = 0;
            //            }

            #endregion

            GameObject targetGameObject = this.Targets[this.targetCounter];
            Transform targetTransform = targetGameObject.transform;

            float distance = Vector3.Distance(this.transform.position, targetTransform.position);
            float trajectoryAngle;

            Vector3 targetCenter = TrajectoryHelper.FirstOrderIntercept(this.transform.position, Vector3.zero,
                this.ShotSpeed, targetTransform.position, targetTransform.GetComponent<Rigidbody>().velocity);

            if (TrajectoryHelper.CalculateTrajectory(distance, this.ShotSpeed, out trajectoryAngle)) {
                float trajectoryHeight = Mathf.Tan(trajectoryAngle * Mathf.Deg2Rad) * distance;
                targetCenter.y += trajectoryHeight;
            }

            //fire at TargetCenter
            BaseProjectile toFire = Instantiate(this.Projectile);
            toFire.Target = targetGameObject; // Set the intended target of the projectile.
            toFire.gameObject.SetActive(true); // Active the projectile (not needed if the prefab is already active).
            toFire.transform.position = this.transform.position; // Set the position to the position of the shooter.
            toFire.transform.LookAt(targetCenter); // Easiest way to get the projectile facing the target.
            toFire.Rigidbody.AddRelativeForce(0, 0, this.ShotSpeed, ForceMode.Impulse);

            if (++this.targetCounter >= this.Targets.Length) {
                // Start from the beginning of the list again.
                this.targetCounter = 0;
            }
        }

        private Vector3 GetRandomnessOfShot() {
            return Random.insideUnitSphere * this.Inaccuracy;
        }
    }
}