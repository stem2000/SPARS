using Avatar;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class AvatarDebugger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _prev;
    [SerializeField] private TextMeshProUGUI _curr;

    [HideInInspector] public StateAutomatRestricted _state;

    public void UpdateStateInfo()
    {
        _prev.text = "PreviousState:" + _curr.text.Replace("CurrentState:", "");
        _curr.text = "CurrentState:" + _state.CurrentMoveState.ToString();
    }

    public void Update()
    {
        if (_state.WasMoveStateChanged)
        {
            UpdateStateInfo();
        }
    }
}
