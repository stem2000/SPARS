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

        [Space]
        [Header("Animations")]
        [SerializeField] private AvatarAnimationController _avatarAnimationController;

        private AvatarState _avatarState;
        private AvatarMovement _avatarMovement;
        private AvatarWorldListener _avatarWorldListener;
        private StateChangingReactor _avatarStateChangingReactor;

        private InputManager _playerInput;

        #region METHODS
        private void HandleInput()
        {
            HandlePlayerInput();
            HandleWorldInput();
        }

        private void ChangeState()
        {
            _avatarState.ChangeState();

            var stateInfo = _avatarState.GetStateInfo();
            _avatarStateChangingReactor.ReactToStateChanging(stateInfo);
            _avatarBeatController.ReactToStateChanging(stateInfo);

            _avatarState.UpdateStats(_avatarStateChangingReactor.GetStatsPackage());
            _avatarMovement.UpdateMovementData(_avatarState.GetMovementData());
        }

        private void HandlePlayerInput()
        {
            var moveDirection = _playerInput.GetPlayerMovement();
            _avatarState.MoveDirection = new Vector3(moveDirection.x, 0f, moveDirection.y);
            _avatarState.ShouldDash = _playerInput.GetDashInput();
            _avatarState.ShouldJump = _playerInput.GetJumpInput();
            _avatarState.ShouldShoot = _playerInput.GetShootInput();
            _avatarState.ShouldPunch = _playerInput.GetPunchInput();

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
            _avatarStateChangingReactor = new StateChangingReactor(GetComponent<Rigidbody>(), _avatarStats.Clone());
            _avatarState = new AvatarState(_avatarStateChangingReactor.GetStatsPackage());
            _avatarMovement = new AvatarMovement(GetComponent<Rigidbody>());
            _avatarWorldListener = GetComponent<AvatarWorldListener>();
            _avatarBeatController.InitializeComponents();
            _avatarAnimationController.SetAnimatorVariablesHashes();
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
            ChangeState();
            Animate();
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
