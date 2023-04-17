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
    }


    public bool IsAvatarGrounded()
    {
        return _isOnGround;
    }
    

    public bool IsAvatarOnSlope()
    {
        if (Physics.Raycast(_rigidbody.transform.position, Vector3.down, out _slopeHit, _rigidbody.transform.lossyScale.y * 0.5f))
        {
            var dot = Vector3.Dot(Vector3.up, _slopeHit.normal);
            if (dot != 1)
                return true;
        }
        return false;
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
