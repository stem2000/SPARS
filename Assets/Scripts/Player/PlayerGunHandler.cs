using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerGunHandler : MonoBehaviour, ActReceiver
{
    [SerializeField] private UnityEvent ShootEvent;

    [HideInInspector] private bool _canShoot = false;


    public bool ReceiveAct(ActType act, bool shouldAct)
    {
        switch(act)
        {
            case ActType.Shoot:
                return HadleShootInput(shouldAct);
            default:
                return false;
        }
    }


    private bool HadleShootInput(bool shouldAct)
    {
        _canShoot = shouldAct;
        return _canShoot;
    }

    public void TryShoot()
    {
        if (_canShoot)
        {
            ShootEvent.Invoke();
            _canShoot = false;
        }
    }


    protected void Update()
    {
        TryShoot();
    }

}
