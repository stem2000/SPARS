using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AvatarRotation
{
    [SerializeField] private Transform _avatarBodyTransform;
    [SerializeField] private Transform _avatarMeshTransform;
    [SerializeField] private Transform _cameraHolderTransform;
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


    public void HandleRotationInput(Vector2 mouseInput)
    {
        Vector2 mouseDelta = mouseInput;
        xRotation += mouseDelta.x * Sensitivity;
        yRotation -= mouseDelta.y * Sensitivity;
        yRotation = Mathf.Clamp(yRotation, -XRotationUpperLimit, -XRotationLowerLimit);
    }


    private void UpdateCameraRotation()
    {
        _cameraHolderTransform.localRotation = Quaternion.Euler(yRotation, xRotation, 0f);
    }


    private void UpdateAvatarRotation()
    {
        _avatarBodyTransform.localRotation = Quaternion.Euler(0.0f, xRotation, 0.0f);
    }

    private void UpdateCameraPosition()
    {
        Vector3 newPos = _avatarBodyTransform.position + GetScaledOffset();
        _cameraHolderTransform.position = newPos;
    }


    private void UpdateMeshPosition()
    {
        _avatarMeshTransform.position = _avatarBodyTransform.position;
    }


    private void UpdateMeshRotation()
    {
        _avatarMeshTransform.localRotation = Quaternion.Euler(0.0f, xRotation, 0.0f);
    }


    private Vector3 GetScaledOffset()
    {
        var cameraForward = new Vector3(_cameraHolderTransform.forward.x, 0f, _cameraHolderTransform.forward.z).normalized;
        Vector3 offset = new Vector3(cameraForward.x * headWidth, Mathf.Lerp(0f, _bodyCollider.height, headHeight), cameraForward.z * headWidth);
        return offset;
    }
}
