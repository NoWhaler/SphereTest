using UnityEngine;

namespace Main.Scripts.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private MainSphere _target;
        [SerializeField] private float _followSpeed = 5.0f;

        private Vector3 _offset;

        private void Start()
        {
            _offset = transform.position - _target.transform.position;
        }

        private void LateUpdate()
        {
            var targetPosition = _target.transform.position + _offset;
            var newPosition = Vector3.Lerp(transform.position, targetPosition, _followSpeed * Time.deltaTime);

            transform.position = newPosition;
        }
    }
}