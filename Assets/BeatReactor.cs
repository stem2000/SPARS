using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface BeatReactor 
{
    public void SetNewBeatState();
    public void UpdateBeatState(float sampleShift);
}
