using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    //Eventos
    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnPaused;

    private enum State
    {
        WaitingToStart,
        GamePlaying,
        GameOver
    }
    private State _currState;
    private float _gamePlayingTimer = 0f;
    private float _gamePlayingTimerMax = 100f;

    private bool _isGamePaused = false;
    private bool _isCursorLocked = false;

    private void Awake()
    {
        Instance = this;
        _currState = State.WaitingToStart;
    }

    private void Start()
    {
        ChangeCursorState();
        InputManager.Instance.OnPauseAction += GameInput_OnPauseAction;
        InputManager.Instance.OnInteractAction += GameInput_OnInteractAction;
    }

    private void Update()
    {
        switch (_currState)
        {
            case State.WaitingToStart:

                break;
            case State.GamePlaying:
                _gamePlayingTimer -= Time.deltaTime;
                if(_gamePlayingTimer <= 0f)
                {
                    _gamePlayingTimer = _gamePlayingTimerMax;
                    _currState = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
        }

        Debug.Log(_currState);
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if(_currState == State.WaitingToStart)
        {
            _currState = State.GamePlaying;
            _gamePlayingTimer = _gamePlayingTimerMax;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsGamePlaying()
    {
        return _currState == State.GamePlaying;
    }
    public bool IsGameOver()
    {
        return _currState == State.GameOver;
    }

    public float GetGamePlayingTimerNormalized()
    {
        if(_currState != State.GamePlaying)
        {
            return 0f;
        }
        return 1 - (_gamePlayingTimer / _gamePlayingTimerMax);
    }

    //Cambia el estado del cursor
    private void ChangeCursorState()
    {
        //si el cursor esta bloqueado, se desbloquea, cursor libre
        if (_isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _isCursorLocked = false;
        }

        //si no esta bloqueado, se bloquea, cursor centrado
        else if (!_isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _isCursorLocked = true;
        }
    }



    public void TogglePauseGame()
    {
        _isGamePaused = !_isGamePaused;
        if (_isGamePaused)
        {
            //pausa
            Time.timeScale = 0f;
            ChangeCursorState();
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            //reanudar
            Time.timeScale = 1f;
            ChangeCursorState();
            OnGameUnPaused?.Invoke(this, EventArgs.Empty);
        }
    }

    //Eventos

    private void GameInput_OnPauseAction (object sender, EventArgs e)
    {
        TogglePauseGame();
    }



}
