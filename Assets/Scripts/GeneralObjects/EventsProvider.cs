using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class EventsProvider : MonoBehaviour
{
    private UnityEvent OnDash;
    private UnityEvent OnJump;
    private UnityEvent OnShoot;
    private UnityEvent OnPunch;
    private UnityEvent OnStartSample;
    private UnityEvent OnUpdateSample;

    public void SubscribeToDashEvent(UnityAction function)
    {
        OnDash.AddListener(function);
    }

    public void SubscribeToJumpEvent(UnityAction function)
    {
        OnJump.AddListener(function);
    }

    public void SubscribeToShootEvent(UnityAction function)
    {
        OnShoot.AddListener(function);
    }

    public void SubscribeToPunchEvent(UnityAction function)
    {
        OnPunch.AddListener(function);
    }

    public void SubscribeToStartSampleEvent(UnityAction function)
    {
        OnStartSample.AddListener(function);
    }

    public void SubscribeToUpdateSampleEvent(UnityAction function)
    {
        OnUpdateSample.AddListener(function);
    }

    public void CancelDashSubscription(UnityAction function)
    {
        OnDash.RemoveListener(function);
    }

    public void CancelJumpSubscription(UnityAction function)
    {
        OnJump.RemoveListener(function);
    }

    public void CancelShootSubscription(UnityAction function)
    {
        OnShoot.RemoveListener(function);
    }

    public void CancelPunchSubscription(UnityAction function)
    {
        OnPunch.RemoveListener(function);
    }

    public void CancelStartSampleSubscription(UnityAction function)
    {
        OnStartSample.RemoveListener(function);
    }

    public void CancelSampleUpdateSubscription(UnityAction function)
    {
        OnUpdateSample.RemoveListener(function);
    }

    [SerializeField] private void InvokeDashEvent()
    {
        OnDash.Invoke();
    }

    [SerializeField] private void InvokeJumpEvent()
    {
        OnJump.Invoke();
    }

    [SerializeField] private void InvokeShootEvent()
    {
        OnShoot.Invoke();
    }

    [SerializeField] private void InvokePunchEvent()
    {
        OnPunch.Invoke();
    }

    [SerializeField] private void InvokeStartSampleEvent()
    {
        OnStartSample.Invoke();
    }

    [SerializeField] private void InvokeUpdateSampleEvent()
    {
        OnUpdateSample.Invoke();
    }

}

