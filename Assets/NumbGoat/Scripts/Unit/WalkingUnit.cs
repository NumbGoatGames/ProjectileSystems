﻿using System.Collections.Generic;
using NumbGoat.ProjectileSystems.Scripts;
using NumbGoat.ProjectileSystems.Scripts.Projectile;
using UnityEngine;
using UnityEngine.AI;

namespace NumbGoat.Unit {
    /// <summary>
    ///     Basic walking unit.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody))]
    public class WalkingUnit : MonoBehaviour, IMoving, IHittable {
        public float CurrentHealth = 100;
        private readonly float HealthRegen = 0.5f;
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
        ///     The velocity of this unit, two options: use navMeshAgent.velocity or lastPosition/secondLastPosition * lastPositionTime.
        /// </summary>
        public Vector3 Velocity => this.navMeshAgent.velocity;

        /// <inheritdoc />
        public void DoHit<T>(T hitWith) where T : BaseProjectile {
            // When we are hit, take damage.
            this.TakeDamage(hitWith.Damage);
        }

        public void Awake() {
            this.navMeshAgent = this.GetComponent<NavMeshAgent>();
            this.rigidBody = this.GetComponent<Rigidbody>();
            this.lastPosition = this.gameObject.transform.position;
            // Combine possible targets into one list we can iterate through.
            foreach (GameObject targetObject in this.TargetObjects) {
                this.Targets.Add(targetObject.transform.position);
            }
        }

        public void Start() {
            // Do some sanity checks before we start.
            if (this.Targets.Count == 0) {
                Debug.LogWarning(message: "No Targets set for Walking unit.");
                this.IsActivelyWalking = false;
            } else if (!this.navMeshAgent.isOnNavMesh) {
                Debug.Log(message: "No nav mesh under walking unit.");
                this.IsActivelyWalking = false;
            }
        }

        /// <summary>
        ///     Updates the color of the unit based on its health.
        ///     Color is in a gradient from red to green, where green is 0% health, and red is 100% health. To indicate hits on targets.
        ///     (Green = good hits)
        /// </summary>
        private void UpdateColor() {
            float healthFraction = this.CurrentHealth / this.MaxHealth;
            float green = 1 - healthFraction;
            float red = healthFraction;
            Color c = new Color(red, green, 0f);
            this.GetComponent<MeshRenderer>().material.color = c;
        }

        public void Update() {
#if UNITY_EDITOR
            this.MyVelocity = this.Velocity; // Show velocity in the editor. Don't do this in a live game. 
#endif
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
                this.CurrentHealth += this.HealthRegen;
            }

            this.UpdateColor();
        }

        /// <summary>
        ///     Make this unit take this amount of damage.
        /// </summary>
        /// <param name="damage">Damage to take</param>
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