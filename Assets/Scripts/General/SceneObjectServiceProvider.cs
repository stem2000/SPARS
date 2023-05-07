using UnityEngine;

public class SceneObjectServiceProvider : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    private static Camera _staticCameraLink;

    protected void Awake()
    {
        _staticCameraLink = _mainCamera;
    }

    public static Vector3 GetScreenCenterDiretion()
    {
        var x = Screen.width / 2;
        var y = Screen.height / 2;
        return _staticCameraLink.ScreenPointToRay(new Vector3(x, y, 0)).direction; ;
    }

}
