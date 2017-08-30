using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NumbGoat.Projectile {
    /// <summary>
    ///     Controller class for testing projectile firing.
    /// </summary>
    public class ProjectileTestFire : MonoBehaviour {
        public BaseProjectile Projectile;
        public bool Running = true;
        public float ShootSpeed = 1f;
        public GameObject[] Targets;
        public float Inaccuracy = 0f;
        public bool UseNegAngle = false;
        private int targetCounter = 0;

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
            GameObject targetGameObject = this.Targets[this.targetCounter];
            //TODO Projectile pool(?)
            Vector3 targetPosition = TrajectoryHelper.InterceptPosition(this.transform.position, Vector3.zero,
                targetGameObject.transform.position, Vector3.zero, this.ShootSpeed);
            targetPosition += this.GetRandomnessOfShot();
            float? angle = TrajectoryHelper.CalculateProjectileAngle(this.gameObject.transform.position,
                targetPosition, this.ShootSpeed, this.UseNegAngle);
            if (angle == null) {
                // if the angle couldn't be calculated, it probably means the target is too far away
                // ie. it is impossible to hit the target with the current distance, projectile velocity and gravity
                Debug.LogWarning(message: "Could not find angle to hit target.");
                return;
            }
            float notNullAngle = angle.Value;
            Debug.Log($"Got firing angle of {notNullAngle}");
            BaseProjectile toFire = Instantiate(this.Projectile);
            toFire.Target = targetGameObject; // Set the intended target of the projectile.
            toFire.gameObject.SetActive(true); // Active the projectile (not needed if the prefab is already active).
            toFire.transform.position = this.transform.position; // Set the position to the position of the shooter.
            toFire.transform.LookAt(targetPosition); // Easiest way to get the projectile facing the target.
            toFire.transform.rotation = Quaternion.Euler(notNullAngle, toFire.transform.eulerAngles.y,
                toFire.transform.eulerAngles.z); // Look up at the correct angle.
            toFire.Rigidbody.velocity = toFire.transform.forward * this.ShootSpeed; // Set the projectiles velocity.

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