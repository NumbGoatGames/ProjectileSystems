namespace NumbGoat.ProjectileSystems.Scripts.Projectile {
    /// <summary>
    ///     Interface for objects that want to know when they are hit by projectiles.
    /// </summary>
    public interface IHittable {
        /// <summary>
        ///     This method should be called when this object is hit by a BaseProjectile object.
        /// </summary>
        /// <typeparam name="T">The type of projectile that hits.</typeparam>
        /// <param name="hitWith">The actual projectile that hits.</param>
        void DoHit<T>(T hitWith) where T : BaseProjectile;
    }
}