using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _bulletSpeed = 60f;
    [SerializeField] private ParticleSystem _destroyEffect;
    [SerializeField] private float _timeBeforeAutoDestroy = 3f;
    private Vector3 _shootDirection;
    private Rigidbody _rigidbody;

    private void DoOnDestroy()
    {
        var effect = Instantiate(_destroyEffect);
        effect.transform.position = transform.position;
        gameObject.SetActive(false);
    }

    private IEnumerator DestroyThisBullet()
    {
        yield return new WaitForSeconds(_timeBeforeAutoDestroy);
        DoOnDestroy();
    }

    protected void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    protected void OnEnable()
    {
        StartCoroutine(DestroyThisBullet());
        _shootDirection = SceneObjectServiceProvider.GetScreenCenterDiretion().normalized;
    }

    protected void FixedUpdate()
    {
        _rigidbody.velocity = _shootDirection * _bulletSpeed;
    }

    protected void OnCollisionEnter(Collision collision)
    {
        DoOnDestroy();
    }
}
