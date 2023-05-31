using Avatar;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class BeatController : BeatReactor
{
    public UnityEvent DoOnShoot;
    public UnityEvent DoOnPunch;
    public UnityEvent DoOnJump;
    public UnityEvent DoOnDash;
    public UnityEvent<BeatAction, float> _OnBeatAction;

    private LocalBeatController _myBeatController;
    private StateInfoProvider _stateInfoProvider;

    public bool CanAttack { get{ return _myBeatController.CanAttackThisSample;} }
    public bool CanMove { get { return _myBeatController.CanMoveThisSample; } }

    public void Initialize(StateInfoProvider state)
    {
        _myBeatController = new LocalBeatController();
        _stateInfoProvider = state;
    }

    public void HandleBeatAction()
    {
        if (CheckMoveState() || CheckAttackState())
            HandlePlayerHit();
        else if (_stateInfoProvider.WasAttemptToChangeState)
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
        return _stateInfoProvider.WasMoveStateChanged && (_stateInfoProvider.CurrentMoveState == MovementType.Jump || _stateInfoProvider.CurrentMoveState == MovementType.Dash);
    }

    private bool CheckAttackState()
    {
        return _stateInfoProvider.WasAttackStateChanged && (_stateInfoProvider.CurrentAttackState != AttackType.Calm);
    }

    private void InvokeAttackEvent()
    {
        switch(_stateInfoProvider.CurrentAttackState)
        {
            case AttackType.Shoot:
                DoOnShoot.Invoke();
                _OnBeatAction.Invoke(BeatAction.Shoot, _myBeatController.LastSampleState);
                break;
            case AttackType.Punch:
                DoOnPunch.Invoke();
                _OnBeatAction.Invoke(BeatAction.Punch, _myBeatController.LastSampleState);
                break;
        }
    }

    private void InvokeMoveEvent()
    {
        switch (_stateInfoProvider.CurrentMoveState) 
        {
            case MovementType.Jump:
                DoOnJump.Invoke();
                _OnBeatAction.Invoke(BeatAction.Jump, _myBeatController.LastSampleState);
                break;
            case MovementType.Dash:
                DoOnDash.Invoke();
                _OnBeatAction.Invoke(BeatAction.Dash, _myBeatController.LastSampleState);
                break;
        }
    }
}

public enum BeatAction
{
    Jump, Dash, Shoot, Punch, Miss
}
