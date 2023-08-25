using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Main.Scripts.ObjectPooling
{
    public class ObstaclesSpawner : ObjectPool<Obstacle>
    {
        [Space]
        [Header("Configuration")]
        [SerializeField] private float _minDistance = 1.0f;

        [field: SerializeField] public List<Obstacle> AllSpawnedObstacles { get; set; } = new List<Obstacle>();

        private void Start()
        {
            SpawnObstacles();
        }

        private void SpawnObstacles()
        {
            var obstacleCollider = GetComponent<Collider>();
            var spawnPositions = GenerateSpawnPositions(obstacleCollider.bounds, _poolSize);

            AllSpawnedObstacles = new List<Obstacle>(_freeObjects);

            for (var i = 0; i < spawnPositions.Count; i++)
            {
                var position = spawnPositions[i];
                var obstacle = AllSpawnedObstacles[i];
        
                obstacle.gameObject.SetActive(true);
                obstacle.transform.position = position;
            }
        }

        private List<Vector3> GenerateSpawnPositions(Bounds bounds, int numPositions)
        {
            var spawnPositions = new List<Vector3>();

            for (var i = 0; i < numPositions; i++)
            {
                var isValidPosition = false;
                var newPosition = Vector3.zero;

                var maxAttempts = 100;
                var attempts = 0;

                while (!isValidPosition && attempts < maxAttempts)
                {
                    newPosition = new Vector3(
                        Random.Range(bounds.min.x, bounds.max.x),
                        Random.Range(bounds.min.y, bounds.max.y),
                        Random.Range(bounds.min.z, bounds.max.z)
                    );

                    isValidPosition = IsPositionValid(newPosition, spawnPositions);
                    attempts++;
                }

                if (isValidPosition)
                {
                    spawnPositions.Add(newPosition);
                }
            }

            return spawnPositions;
        }

        private bool IsPositionValid(Vector3 position, IEnumerable<Vector3> existingPositions)
        {
            return existingPositions.All(existingPosition =>
                !(Vector3.Distance(position, existingPosition) < _minDistance));
        }
    }
}