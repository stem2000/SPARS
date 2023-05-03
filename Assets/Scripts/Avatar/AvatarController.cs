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
        private AvatarStateChangingReactor _avatarStateChangingReactor;

        private InputManager _playerInput;
        private DataForStateChanger _inputPackage;

        #region METHODS
        private void HandleInput()
        {
            HandlePlayerInput();
            HandleWorldInput();
        }

        private void ChangeState()
        {
            _avatarState.ChangeState(_inputPackage);
            ReactToStateChanging();
            _avatarState.UpdateStats(_avatarStateChangingReactor.GetStatsPackage());
            _avatarMovement.UpdateMovementData(_avatarState.GetMovementData());
        }

        private void ReactToStateChanging()
        {
            var stateInfo = _avatarState.GetStateInfo();
            _avatarStateChangingReactor.ReactToStateChanging(stateInfo);
            _avatarBeatController.ReactToStateChanging(stateInfo);
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
            if (_inputPackage.OnSlope)
                _inputPackage.Normal = _avatarWorldListener.GetNormal();
        }

        private void Animate()
        {
            _avatarAnimationController.ChangeAnimationState(_avatarState.GetStateInfo());
        }

        private void InitializeComponents()
        {
            var stats = _avatarStats.Clone();

            _avatarStateChangingReactor = new AvatarStateChangingReactor(GetComponent<Rigidbody>(), stats);
            _avatarState = new AvatarState(_avatarStateChangingReactor.GetStatsPackage());
            _avatarMovement = new AvatarMovement(GetComponent<Rigidbody>());
            _avatarWorldListener = GetComponent<AvatarWorldListener>();
            _avatarBeatController.InitializeComponents();
            _avatarAnimationController.SetAnimatorVariablesHashes();

            _inputPackage = new DataForStateChanger();
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
