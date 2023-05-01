using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _bulletSpeed = 60f;
    [SerializeField] private ParticleSystem _destroyEffect;
    Collider _avatarCollider;

    private Rigidbody _rigidbody;


    private void DoOnDestroy()
    {
        var effect = Instantiate(_destroyEffect);
        effect.transform.position = transform.position;
        Physics.IgnoreCollision(GetComponent<Collider>(), _avatarCollider, false);
        Destroy(this);
    }

    private IEnumerator DestroyThisBullet()
    {
        yield return new WaitForSeconds(5f);
        DoOnDestroy();
    }

    protected void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        StartCoroutine(DestroyThisBullet());
    }

    public void SetIgnoreCollision(Collider collider)
    {
        _avatarCollider = collider;
        Physics.IgnoreCollision(GetComponent<Collider>(), collider, true);
    }

    protected void FixedUpdate()
    {
        _rigidbody.velocity = SceneObjectServiceProvider.GetScreenCenterDiretion().normalized * _bulletSpeed;
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Avatar")
        {
            return;
        }

        DoOnDestroy();
        Destroy(gameObject);
    }
}
