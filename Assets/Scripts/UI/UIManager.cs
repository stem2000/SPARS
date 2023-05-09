using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour, BeatReactor
{
    [SerializeField] private HitInBeatStatus _inBeatStatus;
    [SerializeField] private DashStatus _dashStatus;

    #region BEAR REACTOR METHODS
    public void MoveToNextSample()
    {
        return;
    }

    public void SubscibeToUpdateSampleEvents()
    {
        _inBeatStatus.SubscibeToUpdateSampleEvents();
    }

    public void UpdateCurrentSampleState(float sampleState)
    {
        return;
    }
    #endregion

    #region LINK EVENTS METHODS
    public void SubscribeToBeatActEvent(UnityEvent<BeatAction, float> @event)
    {
        @event.AddListener(_inBeatStatus.GetBeatAction);
    }

    public void SubscribeToDashEvent(UnityEvent<float> @event)
    {
        @event.AddListener(_dashStatus.ResetDashStroke);
    }

    public void LinkJumpEvent()
    {

    }

    public void LinkDashEvent()
    {

    }
    #endregion

    #region MONOBEHAVIOUR METHODS
    void Start()
    {
        SubscibeToUpdateSampleEvents();
    }

    void Update()
    {
        
    }
    #endregion
}
