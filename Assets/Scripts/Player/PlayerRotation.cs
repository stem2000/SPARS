using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class PlayerRotation : MonoBehaviour
{ 
    public Transform Player;
    public Transform PlayerMesh;

    public CapsuleCollider PlayerCollider;

    [Range(0, 1)]
    public float headHeight;
    [Range(0, 1)]
    public float headWidth;

    private InputManager _inputManager;

    public float Sensitivity = 10f;
    public float XRotationUpperLimit = 90f;
    public float XRotationLowerLimit = -90f;

    private float xRotation = 0f;
    private float yRotation = 0f;


    protected void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _inputManager = InputManager.Instance;
    }


    protected void LateUpdate()
    {
        HandlePlayerInput();
        UpdateMeshRotation();
        UpdateMeshPosition();
        UpdateCameraRotation();
        UpdateCameraPosition();
    }

    protected void FixedUpdate()
    {
        UpdatePlayerRotation();
    }


    private void HandlePlayerInput()
    {
        Vector2 mouseDelta = _inputManager.GetMouseDelta();
        xRotation += mouseDelta.x * Sensitivity;
        yRotation -= mouseDelta.y * Sensitivity;
        yRotation = Mathf.Clamp(yRotation, -XRotationUpperLimit, -XRotationLowerLimit);
    }


    private void UpdateCameraRotation()
    {
        transform.localRotation = Quaternion.Euler(yRotation, xRotation, 0f);
    }


    private void UpdatePlayerRotation()
    {
        Player.localRotation = Quaternion.Euler(0.0f, xRotation, 0.0f);
    }

    private void UpdateCameraPosition()
    {
        Vector3 newPos = Player.position + GetScaledOffset();
        transform.position = newPos;
    }


    private void UpdateMeshPosition()
    {
        PlayerMesh.position = Player.position;
    }


    private void UpdateMeshRotation()
    {
        PlayerMesh.localRotation = Quaternion.Euler(0.0f, xRotation, 0.0f);
    }


    private Vector3 GetScaledOffset()
    {
        var cameraForward = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
        Vector3 offset = new Vector3(cameraForward.x * headWidth, Mathf.Lerp(0f, PlayerCollider.height, headHeight), cameraForward.z * headWidth);
        return offset;
    }

}


