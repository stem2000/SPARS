using AvatarModel;
using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AvatarBeatController : BeatReactor
{
    [SerializeField] private UnityEvent ShootEvent;
    [SerializeField] private UnityEvent PunchEvent;
    [SerializeField] private UnityEvent JumpEvent;
    [SerializeField] private UnityEvent DashEvent;

    private LocalBeatController _myBeatController;
    private StateChangingData _packageFromState;

    public void InitializeComponents()
    {
        _myBeatController = new LocalBeatController();
        _packageFromState = new StateChangingData();
    }

    public void ReactToStateChanging(in StateChangingData stateInfo)
    {
        _packageFromState = stateInfo;

        if(CheckMoveState() || CheckAttackState())
            HandlePlayerHit();
    }

    public void MoveToNextSample()
    {
        _myBeatController.CanMoveThisSample = true;
        _myBeatController.CanAttackThisSample = true;
    }

    public void UpdateCurrentSampleState(float sampleState)
    {
        _myBeatController.LastSampleState = sampleState;
    }

    private void HandlePlayerHit()
    {
        if(!_myBeatController.CanAttackThisSample)
            Debug.Log("Cant attack");

        if (_myBeatController.CanMoveThisSample)
            if (CheckMoveState())
            {
                InvokeMoveEvent();
                _myBeatController.CanMoveThisSample = false;
            }
        if (_myBeatController.CanAttackThisSample)
            if (CheckAttackState())
            {
                InvokeAttackEvent();
                _myBeatController.CanAttackThisSample = false;
            }
    }

    private bool CheckMoveState()
    {
        return _packageFromState.MoveStateWasChanged && (_packageFromState.CurrentMoveType == MovementType.Jump ||
        _packageFromState.CurrentMoveType == MovementType.Dash);
    }

    private bool CheckAttackState()
    {
        return _packageFromState.AttackStateWasChanged && (_packageFromState.CurrentAttackType != AttackType.Idle);
    }

    private void InvokeAttackEvent()
    {
        switch(_packageFromState.CurrentAttackType)
        {
            case AttackType.Shoot:
                ShootEvent.Invoke();
                break;
            case AttackType.Punch:
                PunchEvent.Invoke();
                break;
        }
    }

    private void InvokeMoveEvent()
    {
        switch (_packageFromState.CurrentMoveType) 
        {
            case MovementType.Jump:
                JumpEvent.Invoke();
                break;
            case MovementType.Dash:
                DashEvent.Invoke();
                break;
        }
    }
}
