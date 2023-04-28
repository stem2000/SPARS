using UnityEngine;

public class AvatarWorldListener : MonoBehaviour
{
    private bool _isOnGround = false;
    private RaycastHit _slopeHit;
    [HideInInspector] private MonoBehaviour physicBody;

    
    public AvatarWorldListener()
    {
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
            if (Mathf.FloorToInt(dot) != 1)
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
