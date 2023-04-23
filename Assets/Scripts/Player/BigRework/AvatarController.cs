using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AvatarModel
{
    public class AvatarController : MonoBehaviour, BeatReactor
    {
        private AvatarState _avatarState;
        [SerializeField] private AvatarStats _avatarStats;
        private LocalBeatController _myBeatController;
        private InputManager _playerInput;
        private AvatarMovement _avatarMovement;
        private AvatarWorldListener _avatarWorldListener;
        private StateChangesFlagsPackage _flagsPackage;


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


        private void PerformActions()
        {
            PerformMovementActions();
        }


        private void ChangeState()
        {
            _avatarState.ChangeState(_flagsPackage);
        }


        private void HandlePlayerInput()
        {
            _flagsPackage.MoveDirection = _playerInput.GetPlayerMovement();
            _flagsPackage.Rotation = _playerInput.GetMouseDelta();
            _flagsPackage.ShouldDash = _playerInput.GetDashInput();
            _flagsPackage.ShouldJump = _playerInput.GetJumpInput();
            _flagsPackage.ShouldShoot = _playerInput.GetShootInput();
        }


        private void HandleWorldInput()
        {
            _flagsPackage.Grounded = _avatarWorldListener.IsAvatarGrounded();
            _flagsPackage.OnSlope = _avatarWorldListener.IsAvatarOnSlope();
            if (_avatarWorldListener.IsAvatarOnSlope())
            {
                _flagsPackage.OnSlope = true;
                _flagsPackage.Normal = _avatarWorldListener.GetNormal();
            }

        }

        private void ProcessPlayerInput()
        {
            TestGeneralPlayerAccuracy();
        }


        private void PerformMovementActions()
        {
            _avatarMovement.ReceiveMovementData(_avatarState.GetMoveState(), _avatarState.GetMovementData());
            _avatarMovement.Move();
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
            _avatarState = new AvatarState(_avatarStats);
            _avatarMovement = new AvatarMovement(GetComponent<Rigidbody>());
            _avatarWorldListener = GetComponent<AvatarWorldListener>();
            _myBeatController = new LocalBeatController();
            _flagsPackage = new StateChangesFlagsPackage();
        }
        #endregion

        #region MONOBEHAVIOUR METHODS
        protected void Start()
        {
            InitializeComponents();
        }


        protected void Awake()
        {
            _playerInput = InputManager.Instance;
        }


        protected void Update()
        {
            HandleInput();
            ProcessInput();
            ChangeState();
            PerformActions();
        }
        #endregion
    }


    public class StateChangesFlagsPackage
    {
        public Vector3 MoveDirection = Vector3.zero;
        public Vector3 Normal = Vector3.zero;
        public Vector3 Rotation = Vector3.zero;

        public bool Grounded;
        public bool OnSlope;
        public bool ShouldDash;
        public bool ShouldJump;
        public bool ShouldShoot;
    }
}
