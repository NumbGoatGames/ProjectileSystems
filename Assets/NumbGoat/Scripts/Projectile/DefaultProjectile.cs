using NumbGoat.Unit;
using UnityEngine;

namespace NumbGoat.Projectile {
    public class DefaultProjectile : BaseProjectile {
        public new void OnCollisionEnter(Collision c) {
            this.DoCollision(c.gameObject, c);
            ExampleTarget otherTarget = c.gameObject.GetComponent<ExampleTarget>();
            if (otherTarget != null) {
                otherTarget.TakeDamage(this.Damage);
            } else {
                WalkingUnit otherTargetWalking = c.gameObject.GetComponent<WalkingUnit>();
                if (otherTargetWalking != null) {
                    otherTargetWalking.TakeDamage(this.Damage);
                }
            }
        }
    }
}