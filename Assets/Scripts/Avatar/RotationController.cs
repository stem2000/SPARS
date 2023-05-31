using System;
using UnityEngine;

namespace Avatar
{
    [Serializable]
    public class RotationController
    {
        [SerializeField] private Transform _avatarBody;
        [SerializeField] private Transform _avatarMesh;
        [SerializeField] private Transform _cameraHolder;
        [SerializeField] private CapsuleCollider _bodyCollider;

        [Range(0, 1)]
        public float headHeight = 0.87f;
        [Range(0, 1)]
        public float headWidth = 0.225f;

        public float Sensitivity = 0.05f;
        public float XRotationUpperLimit = 80f;
        public float XRotationLowerLimit = -70f;

        private float xRotation = 0f;
        private float yRotation = 0f;


        public void RotateAndMoveCamera()
        {
            UpdateMeshRotation();
            UpdateMeshPosition();
            UpdateCameraRotation();
            UpdateCameraPosition();
        }

        public void RotateAvatar()
        {
            UpdateAvatarRotation();
        }


        public void SetMouseDelta(Vector2 mouseInput)
        {
            Vector2 mouseDelta = mouseInput;
            xRotation += mouseDelta.x * Sensitivity;
            yRotation -= mouseDelta.y * Sensitivity;
            yRotation = Mathf.Clamp(yRotation, -XRotationUpperLimit, -XRotationLowerLimit);
        }


        private void UpdateCameraRotation()
        {
            _cameraHolder.localRotation = Quaternion.Euler(yRotation, xRotation, 0f);
        }


        private void UpdateAvatarRotation()
        {
            _avatarBody.localRotation = Quaternion.Euler(0.0f, xRotation, 0.0f);
        }

        private void UpdateCameraPosition()
        {
            Vector3 newPos = _avatarBody.position + GetScaledOffset();
            _cameraHolder.position = newPos;
        }


        private void UpdateMeshPosition()
        {
            _avatarMesh.position = _avatarBody.position;
        }


        private void UpdateMeshRotation()
        {
            _avatarMesh.localRotation = Quaternion.Euler(0.0f, xRotation, 0.0f);
        }


        private Vector3 GetScaledOffset()
        {
            var cameraForward = new Vector3(_cameraHolder.forward.x, 0f, _cameraHolder.forward.z).normalized;
            Vector3 offset = new Vector3(cameraForward.x * headWidth, Mathf.Lerp(0f, _bodyCollider.height, headHeight), cameraForward.z * headWidth);
            return offset;
        }
    }
}
