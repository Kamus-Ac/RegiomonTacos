using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //Singleton
    public static InputManager Instance { get; private set; }

    //Eventos
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;

    //Input Class
    private GameInput _inputActions;

    private void Awake()
    {
        Instance = this;
        _inputActions = new GameInput();
        _inputActions.Player.Enable();
        _inputActions.Player.Interact.performed += Interact_performed;
        _inputActions.Player.InteractAlternate.performed += InteractAlternate_performed;
        _inputActions.Player.Pause.performed += Pause_performed;

    }

    private void OnDestroy()
    {
        _inputActions.Player.Interact.performed -= Interact_performed;
        _inputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;
        _inputActions.Player.Pause.performed -= Pause_performed;
        _inputActions.Dispose();
    }

    //Funciones

    public Vector2 GetMovementVector()
    {
        Vector2 moveInput = _inputActions.Player.Move.ReadValue<Vector2>();
        return moveInput;
    }

    public Vector2 GetLookVector()
    {
        Vector2 lookInput = _inputActions.Player.Look.ReadValue<Vector2>();
        return lookInput;
    }


    //Funciones que llaman a los eventos

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);

    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

}