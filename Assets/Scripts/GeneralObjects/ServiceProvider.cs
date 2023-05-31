using Avatar;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class ServiceProvider : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private BeatManager _beatManager;
    [SerializeField] private AvatarController _avatar;
    [SerializeField] private EventsProvider _eventsProvider;

    public static EventsProvider EventsProvider;
    public static StatsProvider AvatarStats;

    private static Camera _staticCameraLink;
    private static BeatManager _beatManagerLink;
    private static ServiceProvider _serviceProvider;


    protected void Awake()
    {
        _staticCameraLink = _mainCamera;
        _beatManagerLink = _beatManager;
        EventsProvider = _eventsProvider;

        _serviceProvider = this;

        AvatarStats = _avatar.GetStatsProvider();
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
    public static void RunCoroutine(IEnumerator routine)
    {
        _serviceProvider.StartCoroutine(routine);
    }
}
