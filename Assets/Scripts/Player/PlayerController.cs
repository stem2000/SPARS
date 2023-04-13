using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace RhythmShooter.Controllers
{
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(PlayerAnimator))]
    [RequireComponent(typeof(PlayerRotation))]
    [RequireComponent(typeof(PlayerGunHandler))]

    public class PlayerController : MonoBehaviour, BeatReactor
    {
        [SerializeField] private float _shootHitSegment = 0.2f;
        [SerializeField] private float _dashHitSegment = 0.2f;
        [SerializeField] private float _jumpHitSegment = 0.2f;
        [SerializeField] private float _actEndTime = 1f;

        [SerializeField] private UnityEvent ActInBeat;

        private InputManager _inputManager;
        private PlayerMovement _playerMovement;
        private PlayerAnimator _animator;
        private PlayerGunHandler _gunHandler;

        private Vector2 _playerInput = Vector3.zero;
        private Vector3 _playerInputV3 = Vector3.zero;

        private bool _canActThisBeat = true;

        private float _lastSampleShift;

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
            var shootInnput = _inputManager.GetShootInput();

            _playerInputV3.x = _playerInput.x;
            _playerInputV3.z = _playerInput.y;

            _playerMovement.MoveDirection = _playerInputV3;

            _playerMovement.ShouldJump = IsTimeForAct(jumpInput, _jumpHitSegment);

            _playerMovement.ShouldDash = IsTimeForAct(dashInput, _dashHitSegment);

            _gunHandler.ShouldShoot = IsTimeForAct(shootInnput, _shootHitSegment);
        }



        public void SetNewBeatState()
        {
            _canActThisBeat = true;
        }


        public void UpdateBeatState(float sampleShift)
        {
            _lastSampleShift = sampleShift;
        }


        private bool IsTimeForAct(bool inputTrigger, float _hitSegment)
        {
            if (!inputTrigger)
            {
                return inputTrigger;
            }
            else
            {
                if(_actEndTime - _hitSegment < _lastSampleShift && _canActThisBeat)
                {
                    ActInBeat.Invoke();
                    return true;
                }
                _canActThisBeat = false;
                return false;
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
