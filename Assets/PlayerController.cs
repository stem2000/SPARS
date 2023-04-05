using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RhythmShooter.Controllers
{
    [RequireComponent(typeof(PlayerMovement))] 
    public class PlayerController : MonoBehaviour 
    {
        private InputManager _inputManager;
        private PlayerMovement _playerMovement;

        #region METHODS
        protected void Animate()
        {
            return;
        }

        protected void HandleInput()
        {
            var playerInput = _inputManager.GetPlayerMovement();
            var jumpInput = _inputManager.GetJumpInput();

            _playerMovement.MoveDirection = new Vector3(playerInput.x, 0, playerInput.y );
            _playerMovement.ShouldJump = jumpInput != 0 ? true : false;
        }
        #endregion

        #region MONOBEHAVIOURMETHODS
        protected void Start()
        {
            _playerMovement = GetComponent<PlayerMovement>();
        }

        protected void Awake()
        {
            _inputManager = InputManager.Instance;
        }


        protected void Update()
        {
            HandleInput();
        }

        #endregion
    }
}
