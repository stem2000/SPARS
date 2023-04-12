using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class BeatManager : MonoBehaviour
{
    [SerializeField] private float _bpm;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Intervals[] _intervals;
    [SerializeField] private List<IntervalState> _intervalStates;
    public BeatManager Instance;


    private void Update()
    {
        foreach(Intervals interval in _intervals)
        {
            float sampledTime = (_audioSource.timeSamples) / (_audioSource.clip.frequency * interval.GetIntervalLenght(_bpm));
            interval.CheckForNewInterval(sampledTime);
        }
        CheckForIntervalStateChange();
    }


    public void CheckForIntervalStateChange()
    {
        if(_intervalStates.Count == 0) 
            return;

        var interval = _intervalStates[0];
        var intervalIndex = interval.IntervalToChange;
        if (_intervals[intervalIndex].LastInterval == interval.SampleNumber)
        {
            _intervals[intervalIndex].Steps = interval.NewSteps;
        }
    }


    protected void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}


[System.Serializable]
public class Intervals
{
    [SerializeField] public float Steps;
    [SerializeField] private float _intervalPart = 0.5f;
    [SerializeField] private BeatEvent _beatEvent;
    [SerializeField] private CurrentBeatStateEvent _currentBeatStateEvent;
    private float _intervalLenght;

    [HideInInspector] public int LastInterval;

    public float GetIntervalLenght(float bpm)
    {
        _intervalLenght = 60f / (bpm * Steps);
        return _intervalLenght;
    }


    public void CheckForNewInterval(float sampledTime)
    {
        //Debug.Log($"SampledTime - {sampledTime} LastInterval - {LastInterval}");
        if (Mathf.FloorToInt(sampledTime) != LastInterval)
        {
            LastInterval = Mathf.FloorToInt(sampledTime);           
            _beatEvent.Invoke(_intervalLenght, _intervalPart);
        }
        var beatProgressPoint = sampledTime - LastInterval;
        _currentBeatStateEvent.Invoke(beatProgressPoint);
    }
}


[System.Serializable]
public class IntervalState
{
    public int IntervalToChange;
    public int SampleNumber;
    public float NewSteps;
}


[System.Serializable]
public class BeatEvent: UnityEvent<float, float> { }

[System.Serializable]
public class CurrentBeatStateEvent : UnityEvent<float> { }