using NumbGoat.ProjectileSystems.Scripts.Projectile;
using UnityEngine;

namespace NumbGoat {
    /// <summary>
    ///     Simple implementation of a target.
    /// </summary>
    public class ExampleTarget : MonoBehaviour, IHittable {
        public float CurrentHealth = 100;
        public float HealthRegen = 0.1f;
        public float MaxHealth = 100;

        /// <summary>
        ///     This unit got hit by a BaseProjectile.
        /// </summary>
        /// <param name="projectile">The BaseProjectile that hit us.</param>
        public void DoHit<T>(T projectile) where T : BaseProjectile {
            if (this.CurrentHealth - projectile.Damage <= 0) {
                // Normally do death stuff, here just prevent negative health.
                this.CurrentHealth = 0;
            } else {
                this.CurrentHealth -= projectile.Damage;
            }
        }

        public void FixedUpdate() {
            // Regen health, ensuring it is a valid value.
            if (this.CurrentHealth > this.MaxHealth) {
                this.CurrentHealth = this.MaxHealth;
            } else if (this.CurrentHealth < this.MaxHealth) {
                this.CurrentHealth += this.HealthRegen;
            }

            this.UpdateColor();
        }

        /// <summary>
        ///     Updates the color of the unit based on its health.
        ///     Color is in a gradient from red to green, where green is 100% health, and red is 0% health.
        /// </summary>
        private void UpdateColor() {
            float healthCent = this.CurrentHealth / this.MaxHealth;
            float green = 1 - healthCent;
            float red = healthCent;
            Color c = new Color(red, green, 0f);
            this.GetComponent<MeshRenderer>().material.color = c;
        }
    }
}