using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerGunHandler : MonoBehaviour, BeatReactor
{
    [SerializeField] private float _hitSegment = 0.2f;
    [SerializeField] private float _shootEndTime = 1f;
    [SerializeField] private UnityEvent Shoot;

    [HideInInspector] private bool _canShootThisBeat = false;

    private float _lastSampleShift = 0;


    public void TryShoot()
    {
        Debug.Log($"canShoot - {_canShootThisBeat}, lastSampleShift - {_lastSampleShift}");
        if (_canShootThisBeat && IsTimeForShoot(_lastSampleShift))
        {
            Shoot.Invoke();
        }
        else
        {
            _canShootThisBeat = false;
        }
    }
    

    public void SetNewBeatState()
    {
        _canShootThisBeat = true;
    }


    public void UpdateBeatState(float sampleShift)
    {
        _lastSampleShift = sampleShift;
    }


    private bool IsTimeForShoot(float sampleShift)
    {
        Debug.Log($"shootEndTime - {_shootEndTime}, hitSegment - {_hitSegment}, sampleShift - {sampleShift}");
        return _shootEndTime - _hitSegment < sampleShift;
    }
}
