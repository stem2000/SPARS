using System;
using UnityEngine;
using UnityEngine.Events;

namespace Avatar
{
    [RequireComponent(typeof(WorldListener))]
    public class AvatarController : MonoBehaviour
    {
        [SerializeField] private RotationController _rotationController;
        [SerializeField] private BeatController _beatController;
        [SerializeField] private AnimationController _animationController;
        [SerializeField] private StatsCounter _statsAnalyst;
        [SerializeField] private AvatarDebugger _debugger;

        private StateAutomat _state;
        private StateInfoProvider _stateRestricted;
        private StateHandler _stateHandler;
        private MovementController _moveController;
        private WorldListener _worldInput;
        private StatsProvider _statsProvider;
        private InputManager _playerInput;

        #region METHODS
        private void HandleInput()
        {
            HandlePlayerInput();
            HandleWorldInput();
        }

        private void ChangeState()
        {
            _state.ChangeState();
            _stateHandler.HandleState();
            _moveController.UpdateDirections();
            _beatController.HandleBeatAction();
            _animationController.SwitchAnimation();
        }

        private void HandlePlayerInput()
        {
            var moveDirection = _playerInput.GetPlayerMovement();

            _state.MoveDirection = new Vector3(moveDirection.x, 0f, moveDirection.y);
            _state.ShouldDash = _playerInput.GetDashInput();
            _state.ShouldJump = _playerInput.GetJumpInput();
            _state.ShouldShoot = _playerInput.GetShootInput();
            _state.ShouldPunch = _playerInput.GetPunchInput();
            _state.CanAttack = _beatController.CanAttack;
            _state.CanMove = _beatController.CanMove;

            _rotationController.SetMouseDelta(_playerInput.GetMouseDelta());
        }

        private void HandleWorldInput()
        {
            _state.Grounded = _worldInput.IsAvatarGrounded();
            _state.OnSlope = _worldInput.IsAvatarOnSlope();
            if (_state.OnSlope)
                _state.Normal = _worldInput.GetNormal();
        }

        private void Animate()
        {
            _animationController.UpdateAnimation();
        }

        private void Initialize()
        {
            _worldInput = GetComponent<WorldListener>();

            _statsProvider = new StatsProvider(_statsAnalyst);
            _state = new StateAutomat(_statsProvider);
            _stateRestricted = new StateInfoProvider(_state);
            _moveController = new MovementController(GetComponent<Rigidbody>(), _stateRestricted, _statsProvider);

            _stateHandler = new StateHandler(GetComponent<Rigidbody>(), _stateRestricted, _statsAnalyst);

            _beatController.Initialize(_stateRestricted);
            _animationController.Initialize(_stateRestricted);
            _statsAnalyst.Initialize();

            _playerInput = InputManager.Instance;
            
        }

        private void InitializeDebugger()
        {
            _debugger._state = _stateRestricted;
        }

        public StatsProvider GetStatsProvider()
        {
            return _statsProvider;
        }
        #endregion

        #region MONOBEHAVIOUR METHODS
        protected void Start()
        {
            Initialize();
            SubscibeToBeatEvents();
            InitializeDebugger();
            Cursor.lockState = CursorLockMode.Locked;
        }


        protected void FixedUpdate()
        {
            _moveController.Move();
            _rotationController.RotateAvatar();
        }

        protected void Update()
        {
            HandleInput();
            ChangeState();
            Animate();
        }

        protected void LateUpdate()
        {
            _rotationController.RotateAndMoveCamera();
        }
        #endregion

        public void SubscibeToBeatEvents() 
        {
            _beatController.SubscibeToUpdateSampleEvents();
        }
    }
}
