using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AvatarModel
{
    [RequireComponent(typeof(AvatarMovement))]
    [RequireComponent(typeof(AvatarWorldListener))]
    [RequireComponent(typeof(AvatarState))]
    public class AvatarController : MonoBehaviour, BeatReactor
    {
        private InputManager _playerInput;
        [SerializeField] private LocalBeatController BeatController;
        private AvatarState _avatarState;
        private AvatarMovement _avatarMovement;
        private AvatarWorldListener _avatarWorldListener;
        private StateChangesFlagsPackage _flagsPackage;


        #region BEATREACTOR_METHODS
        public void MoveToNextSample()
        {
            BeatController.CanActThisSample = true;
        }

        public void UpdateCurrentSampleState(float sampleState)
        {
            BeatController.LastSampleState = sampleState;
        }
        #endregion


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
            _avatarMovement.ReceiveMovementData(_avatarState.GetMoveStateType(), _avatarState.GetMovementData());
        }


        private void TestGeneralPlayerAccuracy()
        {

        }


        private bool CheckPlayerHitAccuracy(float hitSegment)
        {
            if (BeatController.SampleActLimit - hitSegment < BeatController.LastSampleState && BeatController.CanActThisSample)
            {
                BeatController.CanActThisSample = false;
                return true;
            }
            BeatController.CanActThisSample = false;
            return false;
        }


        private void InitializeComponents()
        {
            _avatarState = GetComponent<AvatarState>();
            _avatarMovement = GetComponent<AvatarMovement>();
            _avatarWorldListener = GetComponent<AvatarWorldListener>();
            BeatController = new LocalBeatController();
            _flagsPackage = new StateChangesFlagsPackage();
        }


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
    }


    public class StateChangesFlagsPackage
    {
        public Vector3 MoveDirection = Vector3.zero;
        public Vector3 Normal = Vector3.zero;
        public Vector3 Rotation = Vector3.zero;

        public bool Grounded;
        public bool OnSlope;
        public bool DashLocked;
        public bool ShouldDash;
        public bool ShouldJump;
        public bool ShouldShoot;
    }
}
