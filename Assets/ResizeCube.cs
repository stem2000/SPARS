using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeCube : MonoBehaviour
{
    [SerializeField] float _pulseSize = 1.15f;
    private Vector3 _startSize;

    protected void Start()
    {
        _startSize = transform.localScale;
    }

    public void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _startSize, Time.deltaTime * 5f);
    }


    public void Pulse()
    {
        transform.localScale = new Vector3(_startSize.x * _pulseSize, _startSize.y ,_startSize.z * _pulseSize);
    }
}
