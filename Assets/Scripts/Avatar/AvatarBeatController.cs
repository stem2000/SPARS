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
    [SerializeField] private SendBeatActionEvent _sendBeatActionEvent;

    private LocalBeatController _myBeatController;
    private StateChangingData _packageFromState;

    public bool CanAttack { get{ return _myBeatController.CanAttackThisSample;} }
    public bool CanMove { get { return _myBeatController.CanMoveThisSample; } }

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
        else if(_packageFromState.WasAttemptToChangeState)
            _sendBeatActionEvent.Invoke(BeatAction.Miss, _myBeatController.LastSampleState);
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

    public void SubscibeToBeatEvents()
    {
        SceneObjectServiceProvider.SubscribeToBeatStart(MoveToNextSample);
        SceneObjectServiceProvider.SubscribeToBeatUpdate(UpdateCurrentSampleState);
    }

    private void HandlePlayerHit()
    {
        if (CheckMoveState())
        {
            InvokeMoveEvent();
            _myBeatController.CanMoveThisSample = false;
        }
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
                _sendBeatActionEvent.Invoke(BeatAction.Shoot, _myBeatController.LastSampleState);
                break;
            case AttackType.Punch:
                PunchEvent.Invoke();
                _sendBeatActionEvent.Invoke(BeatAction.Punch, _myBeatController.LastSampleState);
                break;
        }
    }

    private void InvokeMoveEvent()
    {
        switch (_packageFromState.CurrentMoveType) 
        {
            case MovementType.Jump:
                JumpEvent.Invoke();
                _sendBeatActionEvent.Invoke(BeatAction.Jump, _myBeatController.LastSampleState);
                break;
            case MovementType.Dash:
                DashEvent.Invoke();
                _sendBeatActionEvent.Invoke(BeatAction.Dash, _myBeatController.LastSampleState);
                break;
        }
    }
}

public enum BeatAction
{
    Jump, Dash, Shoot, Punch, Miss
}

[Serializable]
public class SendBeatActionEvent : UnityEvent<BeatAction, float> { }
