using System;
using UnityEngine;
using UnityEngine.Events;

namespace Avatar
{
    [RequireComponent(typeof(WorldListener))]
    public class AvatarController : MonoBehaviour
    {
        [SerializeField] private RotationController _avatarRotation;
        [SerializeField] private BeatController _beatController;
        [SerializeField] private AnimationController _animationController;
        [SerializeField] private StatsAnalyst _statsAnalyst;

        private StateAutomat _state;
        private StateAutomatRestricted _stateRestricted;
        private StateHandler _stateHandler;
        private MovementController _avatarMovement;
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
            _beatController.HandleBeatAction();
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

            _avatarRotation.HandleInput(_playerInput.GetMouseDelta());
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
            _animationController.ChangeAnimationState();
        }

        private void Initialize()
        {
            _statsProvider = new StatsProvider(_statsAnalyst);
            _state = new StateAutomat(_statsProvider);
            _stateRestricted = new StateAutomatRestricted(_state);
            _avatarMovement = new MovementController(GetComponent<Rigidbody>(), _stateRestricted, _statsProvider);

            _stateHandler = new StateHandler(GetComponent<Rigidbody>(), _stateRestricted, _statsAnalyst);

            _beatController.Initialize(_statsProvider, _stateRestricted);
            _animationController.Initialize(_stateRestricted);
            _statsAnalyst.Initialize();

            _playerInput = InputManager.Instance;
            _worldInput = GetComponent<WorldListener>();
        }

        private void SubscribeUItoEvents()
        {
            ObjectServiceProvider.SubscribeUitoBeatActEvent(_beatController._sendBeatActionEvent);
            ObjectServiceProvider.SubscribeUiToDashEvent(_beatController.DashStartedEvent);
        }
        #endregion

        #region MONOBEHAVIOUR METHODS
        protected void Start()
        {
            Initialize();
            SubscibeToBeatEvents();
            SubscribeUItoEvents();
            Cursor.lockState = CursorLockMode.Locked;
        }


        protected void FixedUpdate()
        {
            _avatarMovement.Move();
            _avatarRotation.RotateAvatar();
        }

        protected void Update()
        {
            HandleInput();
            ChangeState();
            Animate();
        }

        protected void LateUpdate()
        {
            _avatarRotation.RotateAndMoveCamera();
        }
        #endregion

        public void SubscibeToBeatEvents() 
        {
            _beatController.SubscibeToUpdateSampleEvents();
        }
    }
}
