using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerGunHandler : MonoBehaviour
{
    [SerializeField] private UnityEvent ShootEvent;

    [HideInInspector] public bool ShouldShoot = false;


    public void TryShoot()
    {
        if (ShouldShoot)
        {
            ShootEvent.Invoke();
            ShouldShoot = false;
        }
    }


    protected void Update()
    {
        TryShoot();
    }

}
