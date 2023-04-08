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
    [SerializeField] float _lerpSpeed = 50f;


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
        MainCamera.transform.position = transform.position;
        Vector2 mouseDelta = _inputManager.GetMouseDelta();
        xRotation += mouseDelta.x * Sensitivity;
        yRotation -= mouseDelta.y * Sensitivity;
        yRotation = Mathf.Clamp(yRotation, -XRotationUpperLimit, -XRotationLowerLimit);

        var cameraRotation = Quaternion.Euler(yRotation, xRotation, 0f);
        MainCamera.rotation = Quaternion.Lerp(MainCamera.rotation, cameraRotation, _lerpSpeed * Time.deltaTime);
    }


    IEnumerator PostSimulationUpdate()
    {
        YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();
        while (true)
        {
            yield return waitForFixedUpdate;
            _playerRB.MoveRotation(Quaternion.AngleAxis(xRotation, Vector3.up));
        }
    }


}


