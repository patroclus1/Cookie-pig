using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(MouseLook), typeof(PlayerActions))]
public class InputManager : MonoBehaviour
{
    private MouseLook _mouseLook;
    private PlayerActions _actions;

    private Controls _controls;
    private Controls.PlayerGameActions _playerInGame;
    private Vector2 _mouseInput;
    private Vector2 _moveDirection;

    public static InputManager Instance;
    private bool _inputEnabled = true;

    private void Awake()
    {
        SetUpSingleton();

        _mouseLook = GetComponent<MouseLook>();
        _actions = GetComponent<PlayerActions>();
    }

    private void OnEnable()
    {
        _controls = new Controls();
        _playerInGame = _controls.PlayerGame;

        _controls.Enable();
        _playerInGame.MouseLook.performed += _ => SendMouseInput();
        _playerInGame.MoveBody.performed += _ => SendMoveInput();
        _playerInGame.Jump.performed += _ => _actions.PerformJump();
        _playerInGame.Sprint.performed += _ => _actions.EnableSprint();
        _playerInGame.Sprint.canceled += _ => _actions.DisableSprint();

        _playerInGame.CheckpointBind.performed += ctx => _actions.ProbeCheckpoint(ctx);
        _playerInGame.PlatformBind.performed += ctx => _actions.ProbePlatform(ctx);
        _playerInGame.Shoot.performed += _ => _actions.ShootPlatform();
    }

    private void OnDisable()
    {
        _playerInGame.MouseLook.performed -= _ => SendMouseInput();
        _playerInGame.MoveBody.performed -= _ => SendMoveInput();
        _playerInGame.Jump.performed -= _ => _actions.PerformJump();
        _playerInGame.Sprint.performed -= _ => _actions.EnableSprint();
        _playerInGame.Sprint.canceled -= _ => _actions.DisableSprint();

        _playerInGame.CheckpointBind.performed -= ctx => _actions.ProbeCheckpoint(ctx);
        _playerInGame.PlatformBind.performed -= ctx => _actions.ProbePlatform(ctx);
        _playerInGame.Shoot.performed -= _ => _actions.ShootPlatform();
        _controls.Disable();
    }

    private void SetUpSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void DisableInput()
    {
        _inputEnabled = false;
    }

    public void EnableInput()
    {
        _inputEnabled = true;
    }

    private void SendMouseInput()
    {
        if (!_inputEnabled) return;
        _mouseInput = _playerInGame.MouseLook.ReadValue<Vector2>();
        _mouseLook.ReceiveLookInput(_mouseInput);
    }

    private void SendMoveInput()
    {
        _moveDirection = _playerInGame.MoveBody.ReadValue<Vector2>();
        _actions.ReceiveMoveInput(_moveDirection);
    }
}
