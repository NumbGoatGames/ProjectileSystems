using UnityEngine;

namespace NumbGoat {
    /// <summary>
    ///     Interface for an object which moves.
    /// </summary>
    public interface IMoving {
        /// <summary>
        /// Current velocity of this object.
        /// </summary>
        Vector3 Velocity { get; }
    }
}