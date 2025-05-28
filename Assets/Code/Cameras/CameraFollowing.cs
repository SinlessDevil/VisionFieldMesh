using Code.Cameras.Provider;
using UnityEngine;

namespace Code.Cameras
{
    public class CameraFollowing
    {
        private Vector3 _offsetPosition = new(-22.25f, 17.45f, 10f);
        private Vector3 _offsetRotation = new(50f, 0, 0);
        private float _lerpSpeed = 1.5f;

        private Transform _target;
        private ICameraProvider _cameraProvider;

        public CameraFollowing(Transform target, ICameraProvider cameraProvider)
        {
            _target = target;
            _cameraProvider = cameraProvider;
        }

        public void Update()
        {
            if (_target == null || _cameraProvider == null)
                return;

            SetPositionCamera(_target.position, _offsetPosition, _lerpSpeed);
            SetRotationCamera(_offsetRotation);
        }

        private void SetPositionCamera(Vector3 target, Vector3 offset,
            float speed)
        {
            Vector3 targetPos = target + offset;
            _cameraProvider.MainCamera.transform.position = Vector3.Lerp(
                _cameraProvider.MainCamera.transform.position, targetPos, Time.deltaTime * speed);
        }

        private void SetRotationCamera(Vector3 offset)
        {
            _cameraProvider.MainCamera.transform.rotation = Quaternion.Euler(offset);
        }
    }
}