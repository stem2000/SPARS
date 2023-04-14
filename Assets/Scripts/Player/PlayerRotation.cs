using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField] private float _lerpSpeed = 50f;

    public Rigidbody Player;
    public Transform CameraHolder;
    public Transform MainCamera;

    private InputManager _inputManager;

    public float MovementSmoothSpeed = 15f;
    public float Sensitivity = 10f;
    public float XRotationUpperLimit = 90f;
    public float XRotationLowerLimit = -15f;

    private float xRotation = 0f;
    private float yRotation = 0f;


    protected void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _inputManager = InputManager.Instance;
        MainCamera.position = transform.position;

        StartCoroutine(RotateRigidbody());
    }


    protected void LateUpdate()
    {

        Vector2 mouseDelta = _inputManager.GetMouseDelta();
        xRotation += mouseDelta.x * Sensitivity;
        yRotation -= mouseDelta.y * Sensitivity;
        yRotation = Mathf.Clamp(yRotation, -XRotationUpperLimit, -XRotationLowerLimit);

        var cameraRotation = Quaternion.Euler(yRotation, xRotation, 0f);
        MainCamera.rotation = Quaternion.Lerp(MainCamera.rotation, cameraRotation, _lerpSpeed * Time.deltaTime);
        MainCamera.position = CameraHolder.position;
    }


    IEnumerator RotateRigidbody()
    {
        YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();
        while (true)
        {
            yield return waitForFixedUpdate;
            Player.MoveRotation(Quaternion.AngleAxis(xRotation, Vector3.up));
        }
    }

}


