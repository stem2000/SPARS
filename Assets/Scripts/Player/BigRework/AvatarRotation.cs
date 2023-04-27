using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AvatarRotation
{
    private Transform _avatarBodyTr;
    private Transform _avatarMeshTr;
    private Transform _cameraHolderTr;
    private CapsuleCollider _bodyCollider;

    [Range(0, 1)]
    public float headHeight = 0.87f;
    [Range(0, 1)]
    public float headWidth = 0.225f;

    public float Sensitivity = 0.05f;
    public float XRotationUpperLimit = 80f;
    public float XRotationLowerLimit = -70f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    public AvatarRotation(Transform avatarBody, Transform avatarMesh, Transform cameraHolder, CapsuleCollider bodyCollider) 
    {
        _avatarBodyTr = avatarBody;
        _avatarMeshTr = avatarMesh;
        _cameraHolderTr = cameraHolder;
        _bodyCollider = bodyCollider;
    }

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


    public void HandleMouseInput(Vector2 mouseInput)
    {
        Vector2 mouseDelta = mouseInput;
        xRotation += mouseDelta.x * Sensitivity;
        yRotation -= mouseDelta.y * Sensitivity;
        yRotation = Mathf.Clamp(yRotation, -XRotationUpperLimit, -XRotationLowerLimit);
    }


    private void UpdateCameraRotation()
    {
        _cameraHolderTr.localRotation = Quaternion.Euler(yRotation, xRotation, 0f);
    }


    private void UpdateAvatarRotation()
    {
        _avatarBodyTr.localRotation = Quaternion.Euler(0.0f, xRotation, 0.0f);
    }

    private void UpdateCameraPosition()
    {
        Vector3 newPos = _avatarBodyTr.position + GetScaledOffset();
        _cameraHolderTr.position = newPos;
    }


    private void UpdateMeshPosition()
    {
        _avatarMeshTr.position = _avatarBodyTr.position;
    }


    private void UpdateMeshRotation()
    {
        _avatarMeshTr.localRotation = Quaternion.Euler(0.0f, xRotation, 0.0f);
    }


    private Vector3 GetScaledOffset()
    {
        var cameraForward = new Vector3(_cameraHolderTr.forward.x, 0f, _cameraHolderTr.forward.z).normalized;
        Vector3 offset = new Vector3(cameraForward.x * headWidth, Mathf.Lerp(0f, _bodyCollider.height, headHeight), cameraForward.z * headWidth);
        return offset;
    }
}
