using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AvatarModel
{
    [RequireComponent(typeof(AvatarMovement))]
    public class AvatarController : MonoBehaviour, BeatReactor
    {
        private InputManager _playerInput;
        [SerializeField] private AvatarStats StatsOnStart;
        [SerializeField] private LocalBeatController BeatController;
        private AvatarState _avatarState;
        private AvatarMovement _avatarMovement;


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
        }


        private void ProcessInput()
        {
            ProcessPlayerInput();
        }


        private void PerformActions()
        {
            UpdateMovementAction();
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


        private void ProcessPlayerInput()
        {
            TestGeneralPlayerAccuracy();
        }


        private void UpdateMovementAction()
        {
            _avatarMovement.ReceiveMovementData(_avatarState.MovementState.CurrentMoveType, _avatarState.TransferMovementData());
        }


        private void TestGeneralPlayerAccuracy()
        {
            if(_avatarState.ShouldMove.ShouldDash)
                _avatarState.ShouldMove.ShouldDash = CheckPlayerHitAccuracy(_avatarState.AvatarStats.DashHitInterval);
            if (_avatarState.ShouldMove.ShouldJump)
                _avatarState.ShouldMove.ShouldJump = CheckPlayerHitAccuracy(_avatarState.AvatarStats.JumpHitInterval);
            if (_avatarState.ShouldAttack.ShouldShoot)
                _avatarState.ShouldAttack.ShouldShoot = CheckPlayerHitAccuracy(_avatarState.AvatarStats.ShootHitInterval);
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
            _avatarState = new AvatarState(this.gameObject, StatsOnStart);
            _avatarMovement = GetComponent<AvatarMovement>();
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


        private void OnTriggerEnter(Collider other)
        {
            _avatarState.MovementState.Grounded = true;
        }


        private void OnTriggerStay(Collider other)
        {
            _avatarState.MovementState.Grounded = true;
        }

        private void OnTriggerExit(Collider other)
        {
            _avatarState.MovementState.Grounded = false;
        }
    }
}
