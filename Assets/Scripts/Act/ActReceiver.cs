using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ActReceiver 
{
    public bool ReceiveAct(ActType act, bool shouldAct);
}

public enum ActType
{
    Jump, Dash, Shoot
}
