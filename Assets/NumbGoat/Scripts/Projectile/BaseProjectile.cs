using UnityEngine;

namespace NumbGoat.Projectile {
    [RequireComponent(typeof(Rigidbody))]
    public abstract class BaseProjectile : MonoBehaviour {
        public float Damage = 10;
        public float ProjectileLifeSeconds = 600f;
        protected float StartTime;
        internal Rigidbody Rigidbody => this.GetComponent<Rigidbody>();
        public Vector3 MyVelocity = Vector3.zero;

        public GameObject Target;

        public virtual void Start() {
            this.StartTime = Time.time;
        }

        public virtual void Awake() { }

        public virtual void Update() {
            if (this.Target != null) {
                // Look at our target.
//                this.transform.LookAt(this.Target.transform);
            }

            if (Time.time > this.ProjectileLifeSeconds + this.StartTime) {
                // If we have been alive for longer than our ProjectileLifeSeconds.
                Destroy(this.gameObject);
            }
        }

        public virtual void LateUpdate() {
            // UI and Camera Updates.
        }

        public void FixedUpdate() {
            // Physics effects go here.
            this.MyVelocity = this.Rigidbody.velocity;
        }

        public virtual void OnCollisionEnter(Collision c) {
            this.DoCollision(c.gameObject, c);
        }

        public virtual void DoCollision(GameObject other, Collision c) {
            Debug.Log($"Hit {other.name}");
            this.Rigidbody.detectCollisions = false;
            this.Rigidbody.isKinematic = true;
            this.GetComponent<Collider>().enabled = false;
            Destroy(this.gameObject, 2f);
        }
    }
}