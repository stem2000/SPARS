using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private ParticleSystem _flash;
    [SerializeField] private Transform _bulletSpawn;
    [SerializeField] private Bullet _bullet;
    [SerializeField] private int _bulletPoolSize = 20;

     private Bullet[] _bulletsPool;

     private AudioSource _shootSound;

    void Start()
    {
        _flash.Stop();
        _shootSound = GetComponent<AudioSource>();
        FillBulletsPool();
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

    private void FillBulletsPool()
    {
        _bulletsPool = new Bullet[_bulletPoolSize];
        for(int i = 0; i < _bulletPoolSize; i++)
        {
            _bulletsPool[i] = Instantiate(_bullet);
            _bulletsPool[i].transform.position = _bulletSpawn.position;
            _bulletsPool[i].gameObject.SetActive(false);
        }
    }

    private void SpawnBullet()
    {
        for(int i = 0; i < _bulletPoolSize; i++)
        {
            if (!_bulletsPool[i].gameObject.activeSelf)
            {
                _bulletsPool[i].transform.position = _bulletSpawn.position;
                _bulletsPool[i].gameObject.SetActive(true);
                break;
            }
        }
    }
}
