using System;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    private DeliveryBlock _assignedTable;
    private TacoSO _customerOrder;
    private NavMeshAgent agent;
    private Animator animator;
    private bool _hasOrdered;

    public Transform counterPoint;


    enum CustomerState
    {
        WaitingForOrder,
        Insatisfied,
        Angry,
        Eating,
        Happy,
    }

    private CustomerState _currState;
    private CustomerState _lastState;

    private float _waitingForOrderTimerMax = 60;
    private float _waitingForOrderTimer = 0;

    private float _insatisfiedTimerMax = 45;
    private float _insatisfiedTimer = 0;

    private float _eatingTimerMax = 40;
    private float _eatingTimer = 0;

    private void Start()
    {
        TacoManager.Instance.OnRecipeCompleted += TacoManager_OnRecipeCompleted;
        TacoManager.Instance.OnRecipeFailed += TacoManager_OnRecipeFailed;

        agent = GetComponent<NavMeshAgent>();
        counterPoint = GameObject.Find("CounterPoint").transform;
        agent.SetDestination(counterPoint.position);

        _currState = CustomerState.WaitingForOrder;

        animator = GetComponentInChildren<Animator>();
    }


    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!_hasOrdered)
            {
                OrderTaco();
            }
        }

        if (agent.velocity.magnitude >= 0.5f)
        {
            animator.SetFloat("Speed", 1.0f);
        }
        else
        {
            animator.SetFloat("Speed", 0.0f);
        }

        MatchStates();

    }

    private void OnDestroy()
    {
        TacoManager.Instance.OnRecipeCompleted -= TacoManager_OnRecipeCompleted;
        TacoManager.Instance.OnRecipeFailed -= TacoManager_OnRecipeFailed;
    }

    private void MatchStates()
    {
        switch (_currState)
        {
            case CustomerState.WaitingForOrder:

                _waitingForOrderTimer += Time.deltaTime;
                if (_waitingForOrderTimer >= _waitingForOrderTimerMax)
                {
                    Debug.Log("El cliente se ha vuelto insatisfecho");
                    _waitingForOrderTimer = 0;
                    _currState = CustomerState.Insatisfied;

                }
                break;

            case CustomerState.Insatisfied:

                _lastState = _currState;
                _insatisfiedTimer += Time.deltaTime;
                if (_insatisfiedTimer >= _insatisfiedTimerMax)
                {
                    Debug.Log("El cliente se ha enojado");
                    _insatisfiedTimer = 0;
                    _currState = CustomerState.Angry;

                }
                break;

            case CustomerState.Angry:
                Debug.Log("El cliente se ha ido enojado");
                LeaveRestaurant();
                break;

            case CustomerState.Eating:

                _eatingTimer += Time.deltaTime;
                if (_eatingTimer >= _eatingTimerMax)
                {
                    Debug.Log("El cliente esta comiendo");
                    _eatingTimer = 0;
                    _currState = CustomerState.Happy;
                }
                break;

            case CustomerState.Happy:
                Debug.Log("El cliente se ha ido feliz");
                LeaveRestaurant();
                break;
        }
    }


    private void CustomerEating()
    {
        _currState = CustomerState.Eating;

    }

    void OrderTaco()
    {
        Debug.Log("Ordenando...");

        _customerOrder = TacoManager.Instance.GenerateTacoForCustomer(this);

        if (_customerOrder != null)
        {
            _hasOrdered = true;

            _assignedTable.SetTacoSO(_customerOrder);

            Invoke(nameof(GoToTable), 2f);
        }
    }


    void GoToTable()
    {
        agent.SetDestination(_assignedTable.ChooseSeat().position);
    }


    public void AssignTable(DeliveryBlock table)
    {
        _assignedTable = table;

    }

    public DeliveryBlock GetAssignedTable()
    {
        return _assignedTable;
    }


    public TacoSO GetCustomerOrder()
    {
        return _customerOrder;
    }

    private void LeaveRestaurant()
    {

        TacoManager.Instance.RemoveCustomerOrder(this);

        if (_assignedTable != null)
        {
            _assignedTable.IsOccupied = false;
            _assignedTable.SetCustomer(null);
            _assignedTable.SetTacoSO(null);
        }

        StallManager.Instance.GetCurrentCustomers().Remove(this);

        Destroy(gameObject);
    }


    //Eventos
    private void TacoManager_OnRecipeCompleted(object sender, TacoManager.OnRecipeCompletedEventArgs e)
    {
        if (e.customer == this)
        {
            _customerOrder = null;
            CustomerEating();
        }
    }

    private void TacoManager_OnRecipeFailed(object sender, TacoManager.OnRecipeFailedEventArgs e)
    {
        if (e.customer != this)
            return;

        switch (_currState)
        {
            case CustomerState.WaitingForOrder:

                Debug.Log("Cliente inconforme por orden incorrecta");
                _currState = CustomerState.Insatisfied;
                break;

            case CustomerState.Insatisfied:

                Debug.Log("Cliente enojado por segunda orden incorrecta");
                _currState = CustomerState.Angry;
                break;
        }
    }

}
