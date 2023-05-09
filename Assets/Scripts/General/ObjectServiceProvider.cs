using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class ObjectServiceProvider : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private BeatManager _beatManager;
    [SerializeField] private UIManager _uiManager;


    private static Camera _staticCameraLink;
    private static BeatManager _beatManagerLink;
    private static UIManager _uiManagerLink;

    protected void Awake()
    {
        _staticCameraLink = _mainCamera;
        _beatManagerLink = _beatManager;
        _uiManagerLink = _uiManager;
    }

    public static Vector3 GetScreenCenterDiretion()
    {
        var x = Screen.width / 2;
        var y = Screen.height / 2;
        return _staticCameraLink.ScreenPointToRay(new Vector3(x, y, 0)).direction; ;
    }

    #region TOBEAT SUBSCRIBE METHODS
    public static void SubscribeToBeatStart(UnityAction action)
    {
        _beatManagerLink.SubscribeToBeatStart(action);
    }

    public static void SubscribeToBeatUpdate(UnityAction<float> action)
    {
        _beatManagerLink.SubscribeToBeatUpdate(action);
    }
#endregion

    #region UI SUBSCRIBE METHODS
    public static void SubscribeUitoBeatActEvent(UnityEvent<BeatAction, float> @event)
    {
        _uiManagerLink.SubscribeToBeatActEvent(@event);
    }

    public static void SubscribeUiToDashEvent(UnityEvent<float> @event)
    {
        _uiManagerLink.SubscribeToDashEvent(@event);
    }

    #endregion
    public static void RunCoroutine(IEnumerator routine)
    {
        _beatManagerLink.StartCoroutine(routine);
    }
}
