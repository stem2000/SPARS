using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;



public class InputManager : MonoBehaviour
{
    #region FIELDS
    private static InputManager _instance;
    private PlayerControls _playerControls;
    #endregion

    #region PROPERTIES
    public static InputManager Instance
    {
        get { 
            return _instance;
        }
    }
    #endregion

    #region METHODS
    public Vector2 GetPlayerMovement()
    {
        return _playerControls.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetMouseDelta()
    {
        return _playerControls.Player.Look.ReadValue<Vector2>();
    }

    public bool GetJumpInput()
    {
        return _playerControls.Player.Jump.triggered;
    }

    public bool GetDashInput()
    {
        return _playerControls.Player.Dash.triggered;
    }

    public bool GetShootInput()
    {
        return _playerControls.Player.Shoot.triggered;
    }

    public bool GetPunchInput()
    {
        return _playerControls.Player.Punch.triggered;
    }
    #endregion

    #region MONOBEHAVIOUR_METHODS
    protected void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance  = this;
        }
        _playerControls = new PlayerControls();
    }


    protected void OnDisable()
    {
        _playerControls.Disable();
    }


    protected void OnEnable()
    {
        _playerControls.Enable();
    }
    #endregion
}

