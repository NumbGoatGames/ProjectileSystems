using UnityEngine;

namespace NumbGoat.ProjectileSystems.Scripts {
    /// <summary>
    ///     Interface for an object which moves.
    /// </summary>
    public interface IMoving {
        /// <summary>
        ///     Current velocity of this object. Needed to aim at moving objects.
        /// </summary>
        Vector3 Velocity { get; }
    }
}