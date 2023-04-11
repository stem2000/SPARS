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

    #region PROPERITES
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


    public float GetJumpInput()
    {
        return _playerControls.Player.Jump.ReadValue<float>();
    }


    public bool GetDashInput()
    {
        if (_playerControls.Player.Dash.triggered)
        {
            Debug.Log($"Triggered dash");
        }
        return _playerControls.Player.Dash.triggered;
    }
    #endregion

    #region MONOBEHAVIOUR_METHODS
    protected void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
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

