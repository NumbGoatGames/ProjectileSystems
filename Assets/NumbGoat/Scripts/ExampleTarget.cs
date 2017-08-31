using UnityEngine;

namespace NumbGoat {
    public class ExampleTarget : MonoBehaviour {
        public float CurrentHealth = 100;
        public float MaxHealth = 100;

        public void FixedUpdate() {
            if (this.CurrentHealth > this.MaxHealth) {
                this.CurrentHealth = this.MaxHealth;
            } else if (this.CurrentHealth < this.MaxHealth) {
                this.CurrentHealth += 0.1f;
            }

            this.UpdateColor();
        }

        private void UpdateColor() {
            float healthCent = this.CurrentHealth / this.MaxHealth;
            float green = 1 - healthCent;
            float red = healthCent;
            Color c = new Color(red, green, 0f);
            this.GetComponent<MeshRenderer>().material.color = c;
        }

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