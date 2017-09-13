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
                // Target is an ExampleTarget
                otherTarget.TakeDamage(this.Damage);
            } else {
                // Target not an ExampleTarget
                WalkingUnit otherTargetWalking = c.gameObject.GetComponent<WalkingUnit>();
                if (otherTargetWalking != null) {
                    // Target is a WalkingUnit
                    this.gameObject.transform.parent = otherTargetWalking.gameObject.transform;
                    otherTargetWalking.TakeDamage(this.Damage);
                }
            }
        }
    }
}