using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace RhythmShooter.Controllers
{
    [RequireComponent(typeof(PlayerMovement))] 
    public class PlayerController : MonoBehaviour 
    {
        private InputManager _inputManager;
        private PlayerMovement _playerMovement;
        private bool _isRunning;
        private bool _isJumping;
        private int _isRunningHashAnim;
        private int _isJumpingHashAnim;
        private Vector2 _playerInput;
        private Animator _animator;
        #region METHODS
        protected void Animate()
        {
            _animator.SetBool(_isRunningHashAnim, _isRunning);
        }

        protected void HandleInput()
        {
            _playerInput = _inputManager.GetPlayerMovement();
            var jumpInput = _inputManager.GetJumpInput();


            _playerMovement.MoveDirection = new Vector3(_playerInput.x, 0, _playerInput.y );
            _playerMovement.ShouldJump = jumpInput != 0 ? true : false;
        }


        protected void SetUpAnimationTransitions()
        {
            _isRunning = _playerInput.magnitude > 0 && _playerMovement.PlayerIsGrounded() ? true : false;
            _isJumping = !_isRunning;
        }
        #endregion

        #region MONOBEHAVIOURMETHODS
        protected void Start()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _isRunningHashAnim = Animator.StringToHash("isRunning");
            _animator = GetComponent<Animator>();
        }

        protected void Awake()
        {
            _inputManager = InputManager.Instance;
        }


        protected void Update()
        {
            HandleInput();
            SetUpAnimationTransitions();
            Animate();
        }

        #endregion
    }
}
