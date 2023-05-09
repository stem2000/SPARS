using System;
using UnityEngine;
using UnityEngine.Events;

namespace AvatarModel
{
    [RequireComponent(typeof(AvatarWorldListener))]
    public class AvatarController : MonoBehaviour
    {
        [Space]
        [Header("Avatar Stats")]
        [SerializeField] private AvatarStats _avatarStats;

        [Space]
        [Header("Avatar Rotation")]
        [SerializeField] private AvatarRotation _avatarRotation;

        [Space]
        [Header("Beat Events")]
        [SerializeField] private AvatarBeatController _avatarBeatController;

        [Space]
        [Header("Animations")]
        [SerializeField] private AvatarAnimationController _avatarAnimationController;

        private AvatarState _avatarState;
        private AvatarMovement _avatarMovement;
        private AvatarWorldListener _avatarWorldListener;
        private StateChangingHandler _stateChangingHandler;

        private InputManager _playerInput;

        #region METHODS
        private void HandleInput()
        {
            HandlePlayerInput();
            HandleWorldInput();
        }

        private void ChangeState()
        {
            ActualStats actualStats;
            StateData stateData;

            _avatarState.ChangeState();

            stateData = _avatarState.GetStateInfo();

            _stateChangingHandler.GetStateData(stateData);
            _avatarBeatController.GetStateData(stateData);

            actualStats = _stateChangingHandler.GetStatsPackage();

            _avatarState.GetActualStats(actualStats);
            _avatarBeatController.GetActualStats(actualStats);


            _avatarMovement.UpdateMovementData(_stateChangingHandler.GetMovementData());
            _avatarBeatController.HandleBeatAction();
        }

        private void HandlePlayerInput()
        {
            var moveDirection = _playerInput.GetPlayerMovement();
            _avatarState.MoveDirection = new Vector3(moveDirection.x, 0f, moveDirection.y);
            _avatarState.ShouldDash = _playerInput.GetDashInput();
            _avatarState.ShouldJump = _playerInput.GetJumpInput();
            _avatarState.ShouldShoot = _playerInput.GetShootInput();
            _avatarState.ShouldPunch = _playerInput.GetPunchInput();
            _avatarState.CanAttack = _avatarBeatController.CanAttack;
            _avatarState.CanMove = _avatarBeatController.CanMove;

            _avatarRotation.HandleRotationInput(_playerInput.GetMouseDelta());
        }

        private void HandleWorldInput()
        {
            _avatarState.Grounded = _avatarWorldListener.IsAvatarGrounded();
            _avatarState.OnSlope = _avatarWorldListener.IsAvatarOnSlope();
            if (_avatarState.OnSlope)
                _avatarState.Normal = _avatarWorldListener.GetNormal();
        }

        private void Animate()
        {
            _avatarAnimationController.ChangeAnimationState(_avatarState.GetStateInfo());
        }

        private void InitializeComponents()
        {
            _stateChangingHandler = new StateChangingHandler(GetComponent<Rigidbody>(), _avatarStats.Clone());
            _avatarState = new AvatarState(_stateChangingHandler.GetStatsPackage());
            _avatarMovement = new AvatarMovement(GetComponent<Rigidbody>());

            _avatarWorldListener = GetComponent<AvatarWorldListener>();
            _avatarBeatController.InitializeComponents();
            _avatarAnimationController.SetAnimatorVariablesHashes();
            _playerInput = InputManager.Instance;
        }

        private void SubscribeUItoEvents()
        {
            ObjectServiceProvider.SubscribeUitoBeatActEvent(_avatarBeatController._sendBeatActionEvent);
            ObjectServiceProvider.SubscribeUiToDashEvent(_avatarBeatController.DashStartedEvent);
        }
        #endregion

        #region MONOBEHAVIOUR METHODS
        protected void Start()
        {
            InitializeComponents();
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
            _avatarBeatController.SubscibeToUpdateSampleEvents();
        }
    }
}
