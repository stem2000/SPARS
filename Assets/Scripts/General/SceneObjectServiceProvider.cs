using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class SceneObjectServiceProvider : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private BeatManager _beatManager;
    private static Camera _staticCameraLink;
    private static BeatManager _beatManagerLink;

    protected void Awake()
    {
        _staticCameraLink = _mainCamera;
        _beatManagerLink = _beatManager;
    }

    public static Vector3 GetScreenCenterDiretion()
    {
        var x = Screen.width / 2;
        var y = Screen.height / 2;
        return _staticCameraLink.ScreenPointToRay(new Vector3(x, y, 0)).direction; ;
    }

    public static void SubscribeToBeatStart(UnityAction action)
    {
        _beatManagerLink.SubscribeToBeatStart(action);
    }

    public static void SubscribeToBeatUpdate(UnityAction<float> action)
    {
        _beatManagerLink.SubscribeToBeatUpdate(action);
    }

}
