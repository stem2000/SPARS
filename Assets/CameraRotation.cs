using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CameraRotation : MonoBehaviour
{
    public Transform Player;
    public Transform MainCamera;
    private Rigidbody _playerRB;
    public float MovementSmoothSpeed = 15f;
    public float Sensitivity = 10f;
    public float XRotationUpperLimit = 90f;
    public float XRotationLowerLimit = -15f;

    private InputManager _inputManager;
    private float xRotation = 0f;
    private float yRotation = 0f; 

    private Vector3 _nextPosition;
    private Vector3 _cameraHolderLastPosition;
    private Vector3 _interpolatedPosition;
    private Vector3 _positionStepDifference;
    private int _framesSinceLastFixedUpdate = 0;
    private Quaternion _playerRotation;



    private Vector3 InterpolateRigidbodyPosition()
    {
        _interpolatedPosition += _positionStepDifference / (Time.fixedDeltaTime / Time.deltaTime);
        //Debug.Log($"Camera position - {MainCamera.position}, NextPosition - {_nextPosition} InterpolatedPosition - {_interpolatedPosition}, " +
        //            $"Velocity - {_playerRB.velocity}, Position Difference{_interpolatedPosition - MainCamera.position}," +
        //            $"CurrentFrameSinceFixedUpdate - {_framesSinceLastFixedUpdate}");
        return _interpolatedPosition;
    }


    private Vector3 Vector3Clamp(Vector3 value, Vector3 min, Vector3 max)
    {
        var x = Mathf.Clamp(value.x, min.x, max.x);
        var y = Mathf.Clamp(value.y, min.y, max.y);
        var z = Mathf.Clamp(value.z, min.z, max.z);
        var clampedVector = new Vector3(x, y, z);
        return clampedVector;
    }


    protected void Start()
    {
        _playerRB = Player.GetComponent<Rigidbody>();
    }


    protected void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _inputManager = InputManager.Instance;
        MainCamera.position = transform.position;
    }

 
    protected void LateUpdate()
    {
        _framesSinceLastFixedUpdate++;
        Vector2 mouseDelta = _inputManager.GetMouseDelta();
        xRotation += mouseDelta.x * Sensitivity;
        yRotation -= mouseDelta.y * Sensitivity;
        yRotation = Mathf.Clamp(yRotation, -XRotationUpperLimit, -XRotationLowerLimit);

        _playerRotation = Quaternion.Euler(0f, xRotation, 0f);
        var cameraRotation = Quaternion.Euler(yRotation, 0f, 0f);

        //MainCamera.position = transform.position;
        Debug.Log($"Current camera position: {transform.position} Frame number: {Time.frameCount}");
        Debug.Log($"Current player position: {transform.position} Frame number: {Time.frameCount}");

        MainCamera.localRotation = cameraRotation;

    }


    protected void FixedUpdate()
    {
        _playerRB.MoveRotation(_playerRotation);
        _framesSinceLastFixedUpdate = 0;
        _nextPosition = transform.position + _playerRB.velocity * Time.fixedDeltaTime;
        _interpolatedPosition = _cameraHolderLastPosition = transform.position;
        _positionStepDifference = _nextPosition - _interpolatedPosition;
    }
}