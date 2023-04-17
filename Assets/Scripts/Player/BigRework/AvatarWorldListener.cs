using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarWorldListener : MonoBehaviour
{
    private bool _isOnGround = false;
    private Rigidbody _rigidbody;
    private RaycastHit _slopeHit;

    protected void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _slopeHit = new RaycastHit();
    }


    public bool IsAvatarGrounded()
    {
        return _isOnGround;
    }
    

    public bool IsAvatarOnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, transform.lossyScale.y * 0.5f))
        {
            var dot = Vector3.Dot(Vector3.up, _slopeHit.normal);
            if (dot != 1)
                return true;
        }
        return false;
    }


    public Vector3 GetNormal()
    {
        return _slopeHit.normal;
    }


    private void OnTriggerEnter(Collider other)
    {
        _isOnGround = true;
    }


    private void OnTriggerStay(Collider other)
    {
        _isOnGround = true;
    }


    private void OnTriggerExit(Collider other)
    {
        _isOnGround = false;
    }
}
