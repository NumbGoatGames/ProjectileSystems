using UnityEngine;

namespace NumbGoat {
    /// <summary>
    ///     Simple implementation of a target.
    /// </summary>
    public class ExampleTarget : MonoBehaviour {
        public float CurrentHealth = 100;
        public float MaxHealth = 100;

        public void FixedUpdate() {
            // Increment health, ensuring it is a valid value.
            if (this.CurrentHealth > this.MaxHealth) {
                this.CurrentHealth = this.MaxHealth;
            } else if (this.CurrentHealth < this.MaxHealth) {
                this.CurrentHealth += 0.1f;
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