using UnityEngine;

namespace NumbGoat.Projectile {
    [RequireComponent(typeof(Rigidbody))]
    public abstract class BaseProjectile : MonoBehaviour {
        protected bool Collided;
        public float Damage = 50;
        public Vector3 MyVelocity = Vector3.zero;
        public float ProjectileLifeSeconds = 600f;
        protected float StartTime;

        public GameObject Target;
        internal Rigidbody Rigidbody => this.GetComponent<Rigidbody>();

        public virtual void Start() {
            this.StartTime = Time.time;
        }

        public virtual void Awake() {
            this.gameObject.hideFlags = HideFlags.HideInHierarchy;
        }

        public virtual void Update() {
            if (Time.time > this.ProjectileLifeSeconds + this.StartTime) {
                // If we have been alive for longer than our ProjectileLifeSeconds.
                Destroy(this.gameObject);
            }

            if (this.Rigidbody.velocity.magnitude > 1) {
                //Point along the direction we are traveling
                this.transform.rotation = Quaternion.LookRotation(this.Rigidbody.velocity);
            }
        }

        public virtual void LateUpdate() {
            // UI and Camera Updates.
        }

        public void FixedUpdate() {

        }

        public virtual void OnCollisionEnter(Collision c) {
            this.DoCollision(c.gameObject, c);
        }

        public virtual void DoCollision(GameObject other, Collision c) {
            Debug.Log($"Hit {other.name}");
            this.Collided = true;
            this.Rigidbody.detectCollisions = false;
            this.Rigidbody.isKinematic = true;
            this.GetComponent<Collider>().enabled = false;
            this.transform.LookAt(other.transform.root);
            Destroy(this.gameObject, 2f);
        }
    }
}