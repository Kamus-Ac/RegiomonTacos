using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    //Singleton
    public static Player Instance { get; private set; }

    //Events
    public event EventHandler<OnSelectedBlockChangedEventArgs> OnSelectedBlockChanged;
    public class OnSelectedBlockChangedEventArgs : EventArgs
    {
        public BaseBlock selectedBlock;
    }

    public event EventHandler OnKitchenObjectChanged;

    //References
    private CharacterController _controllerInstance;
    [SerializeField] private Transform _playerCamera;
    [SerializeField] private Transform _kitchenObjectHoldPoint;
    private BaseBlock _selectedBlock;
    private KitchenObject _kitchenObject;
    private Animator _animator;

    //Stats
    private Vector2 _mouseDelta;
    private Vector2 _mouseDeltaVelocity;
    private Vector2 _lastInteractionDir;

    private float _moveSpeed = 5f;

    //Mouse FPS
    [Range(0.0f, 0.5f)] public float _mouseSmoothTime = 0.03f;
    private float _mouseSensitivity = 0.5f;
    private float _cameraCap = 0.0f;


    //Bools
    private bool _isMoving = false;
    private bool _isHoldingObject = false;

    // Coroutines
    private Coroutine _speedCoroutine;
    private Coroutine _holdingCoroutine;


    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("There is more than one Player instance in the scene");
        }
        Instance = this;
    }

    private void Start()
    {

        //Suscripcion a los eventos
        InputManager.Instance.OnInteractAction += InputManager_OnInteract;
        InputManager.Instance.OnInteractAlternateAction += InputManager_OnInteractAlternate;
        _controllerInstance = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnDestroy()
    {
        //Desuscripciones de los eventos
        InputManager.Instance.OnInteractAction -= InputManager_OnInteract;
        InputManager.Instance.OnInteractAlternateAction -= InputManager_OnInteractAlternate;
    }

    private void Update()
    {
        Look();
        Movement();
        Interactions();
    }

    private void Look()
    {
        Vector2 inputLookVector = InputManager.Instance.GetLookVector();


        //Muévete hacia este valor objetivo, pero suavemente, aqui MouseDelta sigue calculando cuanto se movio pero suavizado
        //SmoothDamp calcula la velocidad entre frames para mover la posicion hasta el target, mas lejos el target, mas rapido se mueve,
        //pero si esta cerca, se mueve mas lento, hasta llegar al target, la velocidad se va almacenando en MouseDeltaVelocity por referencia
        _mouseDelta = Vector2.SmoothDamp(_mouseDelta, inputLookVector, ref _mouseDeltaVelocity, _mouseSmoothTime);
        //MouseSmoothTime es el tiempo que tarda en llegar al target de forma suave, 
        // entre mas bajo, mas rapido llega,entre mas alto, mas lento llega

        _cameraCap -= _mouseDelta.y * _mouseSensitivity;
        _cameraCap = Mathf.Clamp(_cameraCap, -90.0f, 90.0f);

        _playerCamera.localEulerAngles = Vector3.right * _cameraCap;
        transform.Rotate(Vector3.up * _mouseDelta.x * _mouseSensitivity);
    }

    private void Movement()
    {
        Vector2 inputMovementVector = InputManager.Instance.GetMovementVector();
        inputMovementVector = Vector2.ClampMagnitude(inputMovementVector, 1f);

        if (inputMovementVector == Vector2.zero)
        {
            _isMoving = false;
            if (_speedCoroutine != null) StopCoroutine(_speedCoroutine);
            _speedCoroutine = StartCoroutine(transitionAnimatorSpeedTo(0f));
        }
        else
        {
            _isMoving = true;
            if (_speedCoroutine != null) StopCoroutine(_speedCoroutine);
            _speedCoroutine = StartCoroutine(transitionAnimatorSpeedTo(1f));
        }


        Vector3 moveVector = (transform.forward * inputMovementVector.y + transform.right * inputMovementVector.x) * _moveSpeed;
        _controllerInstance.SimpleMove(moveVector); //framerate independent movement y aplica el grounded auto

    }

    private IEnumerator transitionAnimatorSpeedTo(float speed)
    {
        while (_animator.GetFloat("Speed") != speed)
        {
            _animator.SetFloat("Speed", Mathf.MoveTowards(_animator.GetFloat("Speed"), speed, Time.deltaTime * 5f));
            yield return null;
        }
    }

    private void Interactions()
    {
        Vector2 inputMovementVector = InputManager.Instance.GetMovementVector();
        Vector3 currDir =
        transform.forward * inputMovementVector.y +
        transform.right * inputMovementVector.x;

        if (currDir != Vector3.zero) //if is not moving in any direction, use the last direction to interact
        {
            _lastInteractionDir = currDir;
        }


        Vector3 origin = _playerCamera.position;
        Vector3 direction = _playerCamera.forward;

        float interactDistance = 2.5f;

        if (Physics.Raycast(origin, direction, out RaycastHit raycastHit, interactDistance))
        {
            //Debug.Log(raycastHit.transform.name);
            if (raycastHit.transform.TryGetComponent(out BaseBlock baseBlock))
            {
                
                //Has clear block
                if (baseBlock != _selectedBlock)
                {
                    SetSelectedBlock(baseBlock);
                }
            }
            else
                SetSelectedBlock(null);
        }
        else
            SetSelectedBlock(null);

    }

    private void SetSelectedBlock(BaseBlock selectedBlock)
    {
        this._selectedBlock = selectedBlock;
        OnSelectedBlockChanged?.Invoke(this, new OnSelectedBlockChangedEventArgs
        {
            selectedBlock = _selectedBlock
        });
    }


    public Transform GetKitcheObjectFollowTransform()
    {
        return _kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        _kitchenObject = kitchenObject;
        if (kitchenObject != null)
        {
            _isHoldingObject = true;
            if (_holdingCoroutine != null) StopCoroutine(_holdingCoroutine);
            _holdingCoroutine = StartCoroutine(transitionAnimatorHolding(true));
        }
        else
        {
            _isHoldingObject = false;
            if (_holdingCoroutine != null) StopCoroutine(_holdingCoroutine);
            _holdingCoroutine = StartCoroutine(transitionAnimatorHolding(false));
        }
        OnKitchenObjectChanged?.Invoke(this, EventArgs.Empty);
    }

    public KitchenObject GetKitchenObject()
    {
        return _kitchenObject;
    }

    public void ClearKitchenObject()
    {
        _kitchenObject = null;
        _isHoldingObject = false;
        if (_holdingCoroutine != null) StopCoroutine(_holdingCoroutine);
        _holdingCoroutine = StartCoroutine(transitionAnimatorHolding(false));
        OnKitchenObjectChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool HasKitchenObject()
    {
        return _kitchenObject != null;
    }

    private IEnumerator transitionAnimatorHolding(bool isHolding)
    {
        float currentLayerWeight = _animator.GetLayerWeight(1);
        float targetLayerWeight = isHolding ? 1f : 0f;

        while (currentLayerWeight != targetLayerWeight)
        {
            currentLayerWeight = Mathf.MoveTowards(currentLayerWeight, targetLayerWeight, Time.deltaTime * 5f);
            _animator.SetLayerWeight(1, currentLayerWeight);
            yield return null;
        }
    }


    //Eventos
    private void InputManager_OnInteract(object sender, System.EventArgs e)
    {
        if(!GameManager.Instance.IsGamePlaying()) return;

        if (_selectedBlock != null)
        {
            _selectedBlock.Interact(this);
        }
    }

    private void InputManager_OnInteractAlternate(object sender, System.EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying()) return;

        if (_selectedBlock != null)
        {
            _selectedBlock.InteractAlternate(this);
        }
    }

}
