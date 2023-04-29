using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface BeatReactor 
{
    public void MoveToNextSample();
    public void UpdateCurrentSampleState(float sampleState);
}
