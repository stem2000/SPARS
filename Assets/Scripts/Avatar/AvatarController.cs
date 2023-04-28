using UnityEngine;

namespace AvatarModel
{
    public class AvatarController : MonoBehaviour, BeatReactor
    {
        [SerializeField] private AvatarStats _avatarStats;

        [Space]
        [Header("Avatar Rotation")]
        [SerializeField] private AvatarRotation _avatarRotation;

        private AvatarState _avatarState;
        private AvatarMovement _avatarMovement;
        private AvatarWorldListener _avatarWorldListener;
        private AvatarStateManipulator _avatarStateManipulator;

        private LocalBeatController _myBeatController;
        private InputManager _playerInput;
        private StateUpdatePackage _inputPackage;


        #region BEATREACTOR_METHODS
        public void MoveToNextSample()
        {
            _myBeatController.CanActThisSample = true;
        }

        public void UpdateCurrentSampleState(float sampleState)
        {
            _myBeatController.LastSampleState = sampleState;
        }
        #endregion

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
            _avatarStateManipulator.UpdateAvatarState(_avatarState.GetStateInfo());
            _avatarState.ReceiveUpdatedStateInfo(_avatarStateManipulator.GetStatsPackage());
        }

        private void HandlePlayerInput()
        {
            _inputPackage.MoveDirection = _playerInput.GetPlayerMovement();
            _inputPackage.Rotation = _playerInput.GetMouseDelta();
            _inputPackage.ShouldDash = _playerInput.GetDashInput();
            _inputPackage.ShouldJump = _playerInput.GetJumpInput();
            _inputPackage.ShouldShoot = _playerInput.GetShootInput();
            _avatarRotation.HandleMouseInput(_playerInput.GetMouseDelta());
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

        private void ProcessPlayerInput()
        {
            TestGeneralPlayerAccuracy();
        }

        private void UpdateMovement()
        {
            _avatarMovement.ReceiveMovementData(_avatarState.GetMovementData());
        }

        private void TestGeneralPlayerAccuracy()
        {

        }

        private bool CheckPlayerHitAccuracy(float hitSegment)
        {
            if (_myBeatController.SampleActLimit - hitSegment < _myBeatController.LastSampleState && _myBeatController.CanActThisSample)
            {
                _myBeatController.CanActThisSample = false;
                return true;
            }
            _myBeatController.CanActThisSample = false;
            return false;
        }

        private void InitializeComponents()
        {
            var stats = _avatarStats.Clone();

            _avatarStateManipulator = new AvatarStateManipulator(GetComponent<Rigidbody>(), stats);
            _avatarState = new AvatarState(_avatarStateManipulator.GetStatsPackage());
            _avatarMovement = new AvatarMovement(GetComponent<Rigidbody>());
            _avatarWorldListener = GetComponent<AvatarWorldListener>();

            _myBeatController = new LocalBeatController();
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
            UpdateComponents();
        }

        protected void LateUpdate()
        {
            _avatarRotation.RotateAndMoveCamera();
        }
        #endregion
    }

}