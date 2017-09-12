using NumbGoat.Unit;
using UnityEngine;

namespace NumbGoat.Projectile {
    /// <summary>
    ///     Default implementation of a projectile.
    /// </summary>
    public class DefaultProjectile : BaseProjectile {
        /// <inheritdoc />
        public new void OnCollisionEnter(Collision c) {
            this.DoCollision(c.gameObject, c);
            ExampleTarget otherTarget = c.gameObject.GetComponent<ExampleTarget>();
            if (otherTarget != null) {
                otherTarget.TakeDamage(this.Damage);
            } else {
                WalkingUnit otherTargetWalking = c.gameObject.GetComponent<WalkingUnit>();
                if (otherTargetWalking != null) {
                    this.gameObject.transform.parent = otherTargetWalking.gameObject.transform;
                    otherTargetWalking.TakeDamage(this.Damage);
                }
            }
        }
    }
}