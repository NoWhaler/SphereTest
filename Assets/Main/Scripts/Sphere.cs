using System.Collections;
using Main.Scripts.ObjectPooling;
using UnityEngine;

namespace Main.Scripts
{
    public class Sphere : MonoBehaviour
    {
        [Header("Dependencies")]
        
        [SerializeField] private SmallSpherePool _spherePool;

        [SerializeField] private ObstaclesSpawner _obstaclesSpawner;
        
        [Space]
        [Header("Configuration")]
        
        [SerializeField] private float _pushForce;

        [SerializeField] private float _lifetime;

        [SerializeField] private float _currentLifetime;

        [Space]
        [Header("Colliders")]
        
        [SerializeField] private SphereCollider _collisionCollider;
        
        private Rigidbody _rigidbody;

        private const float BASE_RADIUS = 0.5f;

        private const float EXPLOSION_RADIUS_COEFFICIENT = 0.15f;

        private const float DESTROY_DELAY = 1f;
        
        private void Update()
        {
            CheckForLifeTime();
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            
            _spherePool = FindObjectOfType<SmallSpherePool>();
            _obstaclesSpawner = FindObjectOfType<ObstaclesSpawner>();
        }

        private void OnEnable()
        {
            if (transform.localScale.x == 0f)
            {
                _collisionCollider.radius = 0f;
            }

            _collisionCollider.radius = BASE_RADIUS + transform.localScale.x * EXPLOSION_RADIUS_COEFFICIENT;
            
            _collisionCollider.enabled = false;
            
            PushSphere();
        }

        private void OnDisable()
        {
            _currentLifetime = 0f;
            transform.localScale = Vector3.zero;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.isKinematic = false;
            GetComponent<MeshRenderer>().enabled = true;
        }

        private void CheckForLifeTime()
        {
            if (_currentLifetime < _lifetime)
            {
                _currentLifetime += Time.deltaTime;
            }
            
            else if (_currentLifetime >= _lifetime)
            {
                _spherePool.ReturnToPool(this);
            }
        }
        
        private void PushSphere()
        {
            var forceDirection = Vector3.forward * _pushForce;
            
            _rigidbody.AddForce(forceDirection, ForceMode.Impulse);
        }

        private void OnTriggerEnter(Collider other)
        {
            var obstacle = other.GetComponent<Obstacle>();

            if (obstacle != null)
            {
                _collisionCollider.enabled = true;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            var obstacle = collision.collider.GetComponent<Obstacle>();

            if (obstacle == null) return;

            StartCoroutine(obstacle.ExplosionCo(_obstaclesSpawner, obstacle));
            
            StartCoroutine(DestroySphereCo());
        }

        private IEnumerator DestroySphereCo()
        {
            GetComponent<MeshRenderer>().enabled = false;
            _rigidbody.isKinematic = true;
            
            yield return new WaitForSecondsRealtime(DESTROY_DELAY);
            
            _collisionCollider.enabled = false;
            _spherePool.ReturnToPool(this);
            
        }
    }
}