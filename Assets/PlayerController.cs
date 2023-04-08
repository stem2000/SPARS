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
        private Vector2 _playerInput;
        private Animator _animator;

        #region VARIABLES_FOR_ANIMATION_CONTROL
        private bool _isRunning;
        private bool _isJumping;
        private bool _isGrounded;
        private int _isRunningHash;
        private int _isJumpingHash;
        private int _isGroundedHash;
        private float _xBlendVelocity;
        private float _yBlendVelocity;
        private int _xBlendVelocityHash;
        private int _yBlendVelocityHash;
        [SerializeField]
        private float SmoothAnimationSpeed = 50f;
        #endregion

        #region METHODS
        protected void Animate()
        {
            _animator.SetBool(_isRunningHash, _isRunning);
            _animator.SetBool(_isJumpingHash, _playerMovement.ShouldJump);
            _animator.SetBool(_isGroundedHash, _playerMovement.Grounded);
            _animator.SetFloat(_xBlendVelocityHash, _xBlendVelocity);
            _animator.SetFloat(_yBlendVelocityHash, _yBlendVelocity);
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
            _isRunning = _playerInput.magnitude > 0 ? true : false;
            InterpolateBlendTreeVelocity();
        }
        #endregion

        #region MONOBEHAVIOURMETHODS
        protected void Start()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _animator = GetComponent<Animator>();
            GetAnimatorVariablesHashes();
        }


        protected void GetAnimatorVariablesHashes()
        {
            _isRunningHash = Animator.StringToHash("isRunning");
            _isJumpingHash = Animator.StringToHash("isJumping");
            _isGroundedHash = Animator.StringToHash("isGrounded");
            _xBlendVelocityHash = Animator.StringToHash("xVelocity");
            _yBlendVelocityHash = Animator.StringToHash("yVelocity");
        }


        protected void InterpolateBlendTreeVelocity()
        {
            if (!_isRunning)
            {
                if(_xBlendVelocity > 0 || _yBlendVelocity > 0)
                    _xBlendVelocity = _yBlendVelocity = 0;
                return;
            }

            if(_xBlendVelocity == 0 && _yBlendVelocity == 0)
            {
                _xBlendVelocity = _playerMovement.MoveDirection.x;
                _yBlendVelocity = _playerMovement.MoveDirection.y;
            }
            else
            {
                _xBlendVelocity = Mathf.Lerp(_xBlendVelocity, _playerMovement.MoveDirection.x, SmoothAnimationSpeed * Time.deltaTime);
                _yBlendVelocity = Mathf.Lerp(_yBlendVelocity, _playerMovement.MoveDirection.z, SmoothAnimationSpeed * Time.deltaTime);
            }
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
