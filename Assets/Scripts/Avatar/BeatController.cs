using Avatar;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class BeatController : BeatReactor
{
    public UnityEvent OnShoot;
    public UnityEvent OnPunch;
    public UnityEvent JumpEvent;
    public UnityEvent DashEvent;
    public UnityEvent<BeatAction, float> _OnBeatAction;

    private LocalBeatController _myBeatController;
    private StateAutomatRestricted _state;

    public bool CanAttack { get{ return _myBeatController.CanAttackThisSample;} }
    public bool CanMove { get { return _myBeatController.CanMoveThisSample; } }

    public void Initialize(StateAutomatRestricted state)
    {
        _myBeatController = new LocalBeatController();
        _state = state;
    }

    public void HandleBeatAction()
    {
        if (CheckMoveState() || CheckAttackState())
            HandlePlayerHit();
        else if (_state.WasAttemptToChangeState)
            _OnBeatAction.Invoke(BeatAction.Miss, _myBeatController.LastSampleState);
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

    public void SubscibeToUpdateSampleEvents()
    {
        ServiceProvider.SubscribeToBeatStart(MoveToNextSample);
        ServiceProvider.SubscribeToBeatUpdate(UpdateCurrentSampleState);
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
        return _state.WasMoveStateChanged && (_state.CurrentMoveState == MovementType.Jump || _state.CurrentMoveState == MovementType.Dash);
    }

    private bool CheckAttackState()
    {
        return _state.WasAttackStateChanged && (_state.CurrentAttackState != AttackType.Calm);
    }

    private void InvokeAttackEvent()
    {
        switch(_state.CurrentAttackState)
        {
            case AttackType.Shoot:
                OnShoot.Invoke();
                _OnBeatAction.Invoke(BeatAction.Shoot, _myBeatController.LastSampleState);
                break;
            case AttackType.Punch:
                OnPunch.Invoke();
                _OnBeatAction.Invoke(BeatAction.Punch, _myBeatController.LastSampleState);
                break;
        }
    }

    private void InvokeMoveEvent()
    {
        switch (_state.CurrentMoveState) 
        {
            case MovementType.Jump:
                JumpEvent.Invoke();
                _OnBeatAction.Invoke(BeatAction.Jump, _myBeatController.LastSampleState);
                break;
            case MovementType.Dash:
                DashEvent.Invoke();
                _OnBeatAction.Invoke(BeatAction.Dash, _myBeatController.LastSampleState);
                break;
        }
    }
}

public enum BeatAction
{
    Jump, Dash, Shoot, Punch, Miss
}
