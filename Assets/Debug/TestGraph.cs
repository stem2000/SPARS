using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGraph : MonoBehaviour
{
    [SerializeField] private AnimationCurve Curve;

    private void Update()
    {
        Keyframe keyframe = new Keyframe(Time.time, transform.position.z, 0,0,0,0);
        Curve.AddKey(keyframe);
    }
}
