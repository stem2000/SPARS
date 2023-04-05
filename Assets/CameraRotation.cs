using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public Transform Player;
    public Transform MainCamera;
    public float MovementSmoothSpeed = 15f;
    public float Sensitivity = 10f;
    public float XRotationUpperLimit = 90f;
    public float XRotationLowerLimit = -15f;

    private InputManager _inputManager;
    private float xRotation = 0f;
    private float yRotation = 0f; 
    private Vector3 _velocity = Vector3.zero;


    protected void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _inputManager = InputManager.Instance;
        MainCamera.position = transform.position;
    }

 
    protected void LateUpdate()
    {
        Vector2 mouseDelta = _inputManager.GetMouseDelta();
        xRotation += mouseDelta.x * Sensitivity;
        yRotation -= mouseDelta.y * Sensitivity;
        yRotation = Mathf.Clamp(yRotation, -XRotationUpperLimit, -XRotationLowerLimit);

        var playerRotation = Quaternion.Euler(0f, xRotation, 0f);
        var cameraRotation = Quaternion.Euler(yRotation, xRotation, 0f);

        MainCamera.position = Vector3.Lerp(MainCamera.position, transform.position, MovementSmoothSpeed * Time.deltaTime);


        Player.rotation = playerRotation;
        MainCamera.localRotation = cameraRotation;
    
    }
}