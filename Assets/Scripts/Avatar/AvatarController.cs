using UnityEngine;

namespace AvatarModel
{
    [RequireComponent(typeof(AvatarWorldListener))]
    public class AvatarController : MonoBehaviour, BeatReactor
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

        [SerializeField] private Animator _avatarAnimator;

        private AvatarState _avatarState;
        private AvatarMovement _avatarMovement;
        private AvatarWorldListener _avatarWorldListener;
        private AvatarStateManipulator _avatarStateManipulator;
        private AvatarAnimationController _avatarAnimationController;

        private InputManager _playerInput;
        private StateUpdatePackage _inputPackage;

        #region METHODS
        private void HandleInput()
        {
            HandlePlayerInput();
            HandleWorldInput();
        }

        private void ProcessInput()
        {
            ProcessPlayerInput();
        }

        private void UpdateComponents()
        {
            UpdateMovement();
        }

        private void ChangeState()
        {
            _avatarState.HandleInput(_inputPackage);
            _avatarStateManipulator.GetAndHandleStateInfo(_avatarState.GetStateInfo());
            _avatarBeatController.GetAndHandleStateInfo(_avatarState.GetStateInfo());
            _avatarState.ReceiveUpdatedStateInfo(_avatarStateManipulator.GetStatsPackage());
        }

        private void HandlePlayerInput()
        {
            _inputPackage.MoveDirection = _playerInput.GetPlayerMovement();
            _inputPackage.Rotation = _playerInput.GetMouseDelta();
            _inputPackage.ShouldDash = _playerInput.GetDashInput();
            _inputPackage.ShouldJump = _playerInput.GetJumpInput();
            _inputPackage.ShouldShoot = _playerInput.GetShootInput();
            _inputPackage.ShouldPunch = _playerInput.GetPunchInput();
            _avatarRotation.HandleRotationInput(_playerInput.GetMouseDelta());
        }

        private void HandleWorldInput()
        {
            _inputPackage.Grounded = _avatarWorldListener.IsAvatarGrounded();
            _inputPackage.OnSlope = _avatarWorldListener.IsAvatarOnSlope();
            if (_avatarWorldListener.IsAvatarOnSlope())
            {
                _inputPackage.OnSlope = true;
                _inputPackage.Normal = _avatarWorldListener.GetNormal();
            }

        }

        private void Animate()
        {
            _avatarAnimationController.ChangeAnimationState(_avatarState.GetStateInfo());
        }

        private void ProcessPlayerInput()
        {

        }

        private void UpdateMovement()
        {
            _avatarMovement.ReceiveMovementData(_avatarState.GetMovementData());
        }

        private void InitializeComponents()
        {
            var stats = _avatarStats.Clone();

            _avatarStateManipulator = new AvatarStateManipulator(GetComponent<Rigidbody>(), stats);
            _avatarState = new AvatarState(_avatarStateManipulator.GetStatsPackage());
            _avatarMovement = new AvatarMovement(GetComponent<Rigidbody>());
            _avatarWorldListener = GetComponent<AvatarWorldListener>();
            _avatarBeatController.InitializeComponents();
            _avatarAnimationController = new AvatarAnimationController(_avatarAnimator);

            _inputPackage = new StateUpdatePackage();
        }
        #endregion

        #region MONOBEHAVIOUR METHODS
        protected void Start()
        {
            InitializeComponents();
            Cursor.lockState = CursorLockMode.Locked;
        }

        protected void Awake()
        {
            _playerInput = InputManager.Instance;
        }

        protected void FixedUpdate()
        {
            _avatarMovement.Move();
            _avatarRotation.RotateAvatar();
        }

        protected void Update()
        {
            HandleInput();
            ProcessInput();
            ChangeState();
            Animate();
            UpdateComponents();
        }

        protected void LateUpdate()
        {
            _avatarRotation.RotateAndMoveCamera();
        }

        public void MoveToNextSample()
        {
            _avatarBeatController.MoveToNextSample();
        }

        public void UpdateCurrentSampleState(float sampleState)
        {
            _avatarBeatController.UpdateCurrentSampleState(sampleState);
        }
        #endregion
    }

}
