using System;
using Main.Scripts.ObjectPooling;
using Main.Scripts.UI;
using UnityEngine;

namespace Main.Scripts
{
    public class MainSphere : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float _minimumDecreaseAmount;

        [SerializeField] private float _coefficient;
        
        [SerializeField] private Transform _doorTransform;
        
        [Space]
        [Header("Dependencies")]
        
        [SerializeField] private SmallSpherePool _spherePool;

        [SerializeField] private ObstaclesSpawner _obstaclesSpawner;

        private Vector3 _initialScale;
        private float _accumulatedScale;

        private const float MIN_DISTANCE_TO_OBSTACLE = 5f;

        public event Action onWinLevel;

        public event Action onLoseLevel;

        private void Awake()
        {
            _initialScale = transform.localScale;
        }

        private void OnEnable()
        {
            InputManager.onSphereTap += DecreaseScaleAndSpawnSphereOnTap;
            InputManager.onReleaseTap += DecreaseScaleOnRelease;
            InputManager.onSphereHolding += IncrementScaleOnHolding;
        }

        private void OnDisable()
        {
            InputManager.onSphereTap -= DecreaseScaleAndSpawnSphereOnTap;
            InputManager.onReleaseTap -= DecreaseScaleOnRelease;
            InputManager.onSphereHolding -= IncrementScaleOnHolding;
        }

        private void Update()
        {
            if (_obstaclesSpawner.AllSpawnedObstacles.Count == 0)
            {
                MoveTowardsDoor();
                return;
            }
            
            CheckForClosestObstacle();
        }

        private void SpawnSphere(Vector3 sphereScale)
        {
            var objectToPool = _spherePool.Get();
            
            if (objectToPool == null) return;

            var sphereTransform = objectToPool.transform;
            
            sphereTransform.localScale = sphereScale;
            sphereTransform.position = transform.position;
            objectToPool.gameObject.SetActive(true);
        }
        
        private void IncrementScaleOnHolding(bool isMouseHolding)
        {
            _accumulatedScale = Mathf.Clamp(_accumulatedScale, 0, _initialScale.x);
            
            if (_initialScale.x > _accumulatedScale)
            {
                _accumulatedScale += _minimumDecreaseAmount;
            }
        }

        private void DecreaseScaleOnRelease()
        {
            DecreaseObjectScale(_accumulatedScale);
            
            _accumulatedScale = 0f;
        }
        
        private void DecreaseScaleAndSpawnSphereOnTap()
        {
            if (!(transform.localScale.x >= _minimumDecreaseAmount)) return;

            DecreaseObjectScale(_minimumDecreaseAmount);
        }
        
        private void DecreaseObjectScale(float value)
        {
            transform.localScale -= new Vector3(value, value, value);
            _initialScale -= new Vector3(value, value, value);

            if (_initialScale == Vector3.zero)
            {
                onLoseLevel?.Invoke();
            }

            SpawnSphere(new Vector3(value * _coefficient, value * _coefficient, value * _coefficient));
        }
        
        private void CheckForClosestObstacle()
        {
            var closestDistance = float.MaxValue;
            Obstacle closestObstacle = null;

            foreach (var obstacle in _obstaclesSpawner.AllSpawnedObstacles)
            {
                var distanceToObstacle = Vector3.Distance(transform.position, obstacle.transform.position);

                if (!(distanceToObstacle < closestDistance)) continue;
                
                closestDistance = distanceToObstacle;
                closestObstacle = obstacle;
            }

            if (closestObstacle != null && closestDistance > MIN_DISTANCE_TO_OBSTACLE)
            {
                MoveCloserToObstacle();
            }
        }
        
        private void MoveCloserToObstacle()
        {
            var forwardDirection = transform.forward;

            var movementDirection = new Vector3(forwardDirection.x, 0f, forwardDirection.z);
            var newPosition = transform.position + movementDirection * 2f * Time.deltaTime;

            transform.position = newPosition;
        }
        
        private void MoveTowardsDoor()
        {
            var doorPosition = _doorTransform.position;
            var distanceToDoor = Vector3.Distance(transform.position, doorPosition);

            if (distanceToDoor > 5.0f)
            {
                var forwardDirection = transform.forward;

                var movementDirection = new Vector3(forwardDirection.x, 0f, forwardDirection.z);
                var newPosition = transform.position + movementDirection * 2f * Time.deltaTime;

                transform.position = newPosition;
            }
            else
            {
                onWinLevel?.Invoke();
            }
        }
    }
}