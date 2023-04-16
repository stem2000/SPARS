using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LocalBeatController
{
    [HideInInspector] public bool CanActThisSample = true;
    [HideInInspector] public float LastSampleState = 0f;
    public float SampleActLimit = 1f;
}
