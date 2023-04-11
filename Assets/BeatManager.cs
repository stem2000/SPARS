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
}


[System.Serializable]
public class Intervals
{
    [SerializeField] public float Steps;
    [SerializeField] private UnityEvent _trigger;


    [HideInInspector] public int LastInterval;

    public float GetIntervalLenght(float bpm)
    {
        return 60f / (bpm * Steps);
    }


    public void CheckForNewInterval(float interval)
    {
        if(Mathf.FloorToInt(interval) != LastInterval)
        {
            LastInterval = Mathf.FloorToInt(interval);
            //Debug.Log(LastInterval);
            _trigger.Invoke();
        }
    }
}


[System.Serializable]
public class IntervalState
{
    public int IntervalToChange;
    public int SampleNumber;
    public float NewSteps;
}