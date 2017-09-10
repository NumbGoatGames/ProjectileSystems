using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NumbGoat.Unit {
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody))]
    public class WalkingUnit : MonoBehaviour, IMoving {
        public float CurrentHealth = 100;
        private Vector3 lastPosition;
        private float lastPositionTime;
        public float MaxHealth = 100;
        public Vector3 MyVelocity = Vector3.zero;
        private NavMeshAgent navMeshAgent;
        private int nextTargetIndex;
        private Rigidbody rigidBody;
        private Vector3 secondLastPosition;
        public GameObject[] TargetObjects;
        public List<Vector3> Targets;

        /// <summary>
        ///     True if the unit is currently walking to targets or looking for another target.
        /// </summary>
        public bool IsActivelyWalking { get; set; } = true;

        /// <summary>
        ///     The velocity of this unit, two options: work out yourself using this example, or use navMeshAgent.velocity.
        /// </summary>
//        public Vector3 Velocity => (this.lastPosition - this.secondLastPosition) / this.lastPositionTime;
        public Vector3 Velocity => this.navMeshAgent.velocity;

        public void Awake() {
            this.navMeshAgent = this.GetComponent<NavMeshAgent>();
            this.rigidBody = this.GetComponent<Rigidbody>();
            this.lastPosition = this.gameObject.transform.position;
            foreach (GameObject targetObject in this.TargetObjects) {
                // Combine possible targets.
                this.Targets.Add(targetObject.transform.position);
            }
        }

        public void Start() {
            if (this.Targets.Count == 0) {
                Debug.LogWarning(message: "No Targets set for Walking unit.");
                this.IsActivelyWalking = false;
                return;
            }
            if (!this.navMeshAgent.isOnNavMesh) {
                Debug.Log(message: "No nav mesh under walking unit.");
                this.IsActivelyWalking = false;
            }
        }

        /// <summary>
        ///     Updates the color of the unit based on its health.
        /// </summary>
        private void UpdateColor() {
            float healthCent = this.CurrentHealth / this.MaxHealth;
            float green = 1 - healthCent;
            float red = healthCent;
            Color c = new Color(red, green, 0f);
            this.GetComponent<MeshRenderer>().material.color = c;
        }

        public void Update() {
            this.MyVelocity = this.Velocity; // Show velocity in the editor.
            if (!this.IsActivelyWalking) {
                return;
            }
            if (this.navMeshAgent.stoppingDistance >= this.navMeshAgent.remainingDistance) {
                // We have reached the target. Kinda (could be still doing rotation towards it).
                // Get new target.
                Vector3 nextTarget = this.Targets[this.nextTargetIndex];
                this.navMeshAgent.SetDestination(nextTarget);

                if (++this.nextTargetIndex >= this.Targets.Count) {
                    this.nextTargetIndex = 0;
                }
            }
        }

        public void FixedUpdate() {
            // Variables for calculating velocity.
            this.secondLastPosition = this.lastPosition;
            this.lastPosition = this.gameObject.transform.position;
            this.lastPositionTime = Time.fixedDeltaTime;

            // Regen health, could do in a co-routine or Update() but here makes it easiest to read.
            if (this.CurrentHealth > this.MaxHealth) {
                this.CurrentHealth = this.MaxHealth;
            } else if (this.CurrentHealth < this.MaxHealth) {
                this.CurrentHealth += 0.5f;
            }

            this.UpdateColor();
        }

        /// <summary>
        ///     Make this unit take this amount of damage.
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(float damage) {
            if (this.CurrentHealth - damage <= 0) {
                // Normally do death stuff, here just prevent negative health.
                this.CurrentHealth = 0;
            } else {
                this.CurrentHealth -= damage;
            }
        }
    }
}