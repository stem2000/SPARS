using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace RhythmShooter.Controllers
{
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerAnimator))]
    [RequireComponent(typeof(PlayerRotation))]
    [RequireComponent(typeof(PlayerGunHandler))]

    public class PlayerController : MonoBehaviour 
    {
        private InputManager _inputManager;
        private PlayerMovement _playerMovement;
        private PlayerAnimator _animator;
        private PlayerGunHandler _gunHandler;

        private Vector2 _playerInput = Vector3.zero;
        private Vector3 _playerInputV3 = Vector3.zero;


        #region METHODS
        protected void Animate()
        {
            _animator.Animate(_playerInput.magnitude > 0 ? true : false, 
                              _playerMovement.ShouldJump, 
                              _playerMovement.Grounded, 
                              _playerMovement.MoveDirection);
        }

        protected void HandleInput()
        {
            _playerInput = _inputManager.GetPlayerMovement();
            var jumpInput = _inputManager.GetJumpInput();
            var dashInput = _inputManager.GetDashInput();

            _playerInputV3.x = _playerInput.x;
            _playerInputV3.z = _playerInput.y;

            _playerMovement.MoveDirection = _playerInputV3;

            _playerMovement.ShouldJump = jumpInput != 0 ? true : false;

            _playerMovement.ShouldDash = dashInput;

            ReferToGunHandler(_inputManager.GetShootInput());
        }


        protected void ReferToGunHandler(bool canShoot)
        {
            if (canShoot)
            {
                _gunHandler.TryShoot();
            }
        }

        #endregion

        #region MONOBEHAVIOURMETHODS
        protected void Start()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _animator = GetComponent<PlayerAnimator>();
            _gunHandler = GetComponent<PlayerGunHandler>();
        }


        protected void Awake()
        {
            _inputManager = InputManager.Instance;
        }


        protected void Update()
        {
            HandleInput();
            Animate();
        }
        #endregion
    }
}
