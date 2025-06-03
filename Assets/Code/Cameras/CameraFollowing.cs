using Code.Cameras.Provider;
using UnityEngine;

namespace Code.Cameras
{
    public class CameraFollowing
    {
        private Vector3 _offsetLocal = new(0, 12, -7);
        private Vector3 _fixedEuler = new(35f, 0, 0);
        private float _lerpSpeed = 5f;

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

            UpdatePosition();
            UpdateRotation();
        }

        private void UpdatePosition()
        {
            Transform cam = _cameraProvider.MainCamera.transform;
            
            Vector3 desiredPos = _target.TransformPoint(_offsetLocal);

            cam.position = Vector3.Lerp(
                cam.position,
                desiredPos,
                Time.deltaTime * _lerpSpeed
            );
        }

        private void UpdateRotation()
        {
            Transform cam = _cameraProvider.MainCamera.transform;
            
            Quaternion targetRot = Quaternion.Euler(_fixedEuler.x, _target.eulerAngles.y, _fixedEuler.z);

            cam.rotation = Quaternion.Slerp(
                cam.rotation,
                targetRot,
                Time.deltaTime * _lerpSpeed
            );
        }
    }
}