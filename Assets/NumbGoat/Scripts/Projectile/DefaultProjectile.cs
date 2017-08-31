using UnityEngine;

namespace NumbGoat.Projectile {
    public class DefaultProjectile : BaseProjectile {
        public new void OnCollisionEnter(Collision c) {
            base.DoCollision(c.gameObject, c);
            ExampleTarget otherTarget = c.gameObject.GetComponent<ExampleTarget>();
            if (otherTarget != null) {
                otherTarget.TakeDamage(this.Damage);
            }
        }
    }
}