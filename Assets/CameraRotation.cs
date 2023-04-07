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
    private Quaternion _cameraRotation;


    protected void Start()
    {
        _playerRB = Player.GetComponent<Rigidbody>();
    }


    protected void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _inputManager = InputManager.Instance;
        MainCamera.position = transform.position;
        StartCoroutine(PostSimulationUpdate());
    }

 
    protected void LateUpdate()
    {
        _framesSinceLastFixedUpdate++;
        Vector2 mouseDelta = _inputManager.GetMouseDelta();
        yRotation += mouseDelta.x * Sensitivity;
        xRotation -= mouseDelta.y * Sensitivity;
        xRotation = Mathf.Clamp(xRotation, -XRotationUpperLimit, -XRotationLowerLimit);

        _playerRotation = Quaternion.Euler(0f, yRotation, 0f);
        _cameraRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //MainCamera.position = transform.position;
        Debug.Log($"Current camera position: {transform.position} Frame number: {Time.frameCount}");
        Debug.Log($"Current player position: {transform.position} Frame number: {Time.frameCount}");
    }


    protected void FixedUpdate()
    {
        _framesSinceLastFixedUpdate = 0;
    }


    IEnumerator PostSimulationUpdate()
    {
        YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();
        while (true)
        {
            yield return waitForFixedUpdate;

            Player.GetComponent<Rigidbody>().MoveRotation(_playerRotation.normalized);
            MainCamera.localRotation = _cameraRotation.normalized;

        }
    }


}


