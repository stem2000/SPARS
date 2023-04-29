using AvatarModel;
using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AvatarBeatController : BeatReactor
{
    [SerializeField] private UnityEvent ShootEvent;
    [SerializeField] private UnityEvent PunchEvent;
    [SerializeField] private UnityEvent MoveEvent;

    private LocalBeatController _myBeatController;
    private StateFromInfoPackage _packageFromState;
   

    public void InitializeComponents()
    {
        _myBeatController = new LocalBeatController();
        _packageFromState = new StateFromInfoPackage();
    }

    public void GetAndHandleStateInfo(in StateFromInfoPackage stateInfo)
    {
        _packageFromState = stateInfo;

        if(CheckMoveState())
        {
            HandlePlayerHit();
        }
        else if(CheckAttackState())
        {
            HandlePlayerHit(); 
        }
    }

    public void MoveToNextSample()
    {
        _myBeatController.CanActThisSample = true;
    }

    public void UpdateCurrentSampleState(float sampleState)
    {
        _myBeatController.LastSampleState = sampleState;
    }

    public void HandlePlayerHit()
    {
        if (_myBeatController.CanActThisSample)
        {
            _myBeatController.CanActThisSample = false;
            ShootEvent.Invoke();
            //PunchEvent.Invoke();
            //MoveEvent.Invoke();
        }
    }

    public bool CheckMoveState()
    {
        return _packageFromState.MoveStateWasChanged && (_packageFromState.CurrentMoveType == MovementType.Jump ||
        _packageFromState.CurrentMoveType == MovementType.Dash);
    }

    public bool CheckAttackState()
    {
        Debug.Log($"Attack State Was Changed - {_packageFromState.AttackStateWasChanged}");
        Debug.Log($"Attack State Current Type - {_packageFromState.CurrentAttackType}");
        return _packageFromState.AttackStateWasChanged && (_packageFromState.CurrentAttackType != AttackType.Idle);
    }
}
