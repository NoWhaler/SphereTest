using System.Collections;
using Main.Scripts.ObjectPooling;
using UnityEngine;

namespace Main.Scripts
{
    public class Obstacle : MonoBehaviour
    {
        [field: SerializeField] public ParticleSystem ExplosionParticle { get; set; }

        [SerializeField] private Material _explosiveMaterial;

        public IEnumerator ExplosionCo(ObstaclesSpawner obstaclesSpawner, Obstacle obstacle)
        {
            GetComponent<MeshRenderer>().material = _explosiveMaterial;
            
            yield return new WaitForSecondsRealtime(0.5f);
            
            obstaclesSpawner.AllSpawnedObstacles.Remove(obstacle);
            Instantiate(obstacle.ExplosionParticle, obstacle.transform.position, Quaternion.identity);
            
            obstaclesSpawner.ReturnToPool(obstacle);
        }
    }
}