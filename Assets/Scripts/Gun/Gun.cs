using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private ParticleSystem _flash;
    [SerializeField] private Transform _bulletSpawn;
    [SerializeField] private Bullet _bullet;

    [SerializeField] private Collider _avatarCollider;

     private AudioSource _shootSound;

    void Start()
    {
        _flash.Stop();
        _shootSound = GetComponent<AudioSource>();
    }

    public void Shoot()
    {
        PlayFlash();
        PlaySound();
        SpawnBullet();
    }

    private void PlayFlash()
    {
        if (_flash.isPlaying)
            _flash.Stop();
        if (!_flash.gameObject.activeSelf)
            _flash.gameObject.SetActive(true);
        _flash.Play();
    }
    
    private void PlaySound()
    {
        _shootSound.Play();
    }

    private void SpawnBullet()
    {
        Instantiate(_bullet, _bulletSpawn).SetIgnoreCollision(_avatarCollider);
    }
}
