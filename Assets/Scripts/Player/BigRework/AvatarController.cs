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

        }


        private void HandlePlayerInput()
        {
            _avatarState.MoveDirection = _playerInput.GetPlayerMovement();
            _avatarState.Rotation = _playerInput.GetMouseDelta();
            _avatarState.ShouldMove.ShouldDash = _playerInput.GetDashInput();
            _avatarState.ShouldMove.ShouldJump = _playerInput.GetJumpInput();
            _avatarState.ShouldAttack.ShouldShoot = _playerInput.GetShootInput();
        }


        private void HandleWorldInput()
        {
            _avatarState.Grounded = _avatarWorldListener.IsAvatarGrounded();
            _avatarState.OnSlope = _avatarWorldListener.IsAvatarOnSlope();
            if(_avatarState.OnSlope)
                _avatarState.Normal = _avatarWorldListener.GetNormal();
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
            if(_avatarState.ShouldMove.ShouldDash)
                _avatarState.ShouldMove.ShouldDash = CheckPlayerHitAccuracy(_avatarState.MutableStats.DashHitInterval);
            if (_avatarState.ShouldMove.ShouldJump)
                _avatarState.ShouldMove.ShouldJump = CheckPlayerHitAccuracy(_avatarState.MutableStats.JumpHitInterval);
            if (_avatarState.ShouldAttack.ShouldShoot)
                _avatarState.ShouldAttack.ShouldShoot = CheckPlayerHitAccuracy(_avatarState.MutableStats.ShootHitInterval);
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
            _avatarWorldListener = new AvatarWorldListener();
            BeatController = new LocalBeatController();
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
}
