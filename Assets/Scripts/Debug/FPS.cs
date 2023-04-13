using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{
    [SerializeField] private int fps = 60;

    protected void OnValidate()
    {
        Application.targetFrameRate = fps;
    }
}
