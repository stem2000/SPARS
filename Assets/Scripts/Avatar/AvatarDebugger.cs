using Avatar;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class AvatarDebugger : MonoBehaviour
{
    public TextMeshProUGUI _prev;
    public TextMeshProUGUI _curr;
    public TextMeshProUGUI _fps;

    public int TargetFrameRate;

    [HideInInspector] public StateInfoProvider _state;

    public void UpdateStateInfo()
    {
        _prev.text = "PreviousState:" + _curr.text.Replace("CurrentState:", "");
        _curr.text = "CurrentState:" + _state.CurrentMoveState.ToString();
    }

    public void UpdateFpsInfo()
    {
        _fps.text = "FPS:" + (1f/Time.deltaTime).ToString("F0");
    }

    public void Start()
    {
        Application.targetFrameRate = TargetFrameRate;
    }

    public void Update()
    {
        if (_state.WasMoveStateChanged)
            UpdateStateInfo();
        UpdateFpsInfo();
    }
}
