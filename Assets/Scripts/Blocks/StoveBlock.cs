using System;
using System.Collections;
using UnityEngine;


public class StoveBlock : BaseBlock//, IHasProgress
{
    //public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    private enum State
    {
        Idle,
        Cooking,
        Ready,
        Burned
    }

    [SerializeField] private CookingProcessSO[] _cookingProcessSOArray;
    [SerializeField] private BurningProcessSO[] burningProcessSOArray;

    private float _burningTimer;
    private float _fryingTimer;

    private BurningProcessSO _burningProcessSO;
    private CookingProcessSO _cookingProcessSO;
    private State _currState;

    private void Start()
    {
        _currState = State.Idle;
    }

    private void Update()
    {

        if (HasKitchenObject())
        {

            switch (_currState)
            {
                case State.Idle:
                    break;

                case State.Cooking:
                    _fryingTimer += Time.deltaTime;

                    //OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    //{
                    //    progressNormalized = _fryingTimer / _cookingProcessSO.cookingTimerMax
                    //});

                    if (_fryingTimer > _cookingProcessSO.cookingTimerMax)
                    {
                        //Frying is complete

                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(_cookingProcessSO.output, this);

                        _currState = State.Ready;
                        _burningTimer = 0f;
                        _burningProcessSO = GetBurningProcessSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    }

                    break;
                case State.Ready:
                    _burningTimer += Time.deltaTime;


                    //OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    //{ 
                    //    progressNormalized = _burningTimer / _burningRecipeSO.burningTimerMax
                    //});

                    if (_burningTimer > _burningProcessSO.burningTimerMax)
                    {
                        //Burning is complete

                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(_burningProcessSO.output, this);

                        _currState = State.Burned;

                        //OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        //{
                        //    progressNormalized = 0f
                        //});



                    }
                    break;
                case State.Burned:
                    break;
            }
        }
    }


    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            //No kitchen object here
            if (player.HasKitchenObject())
            {
                //Player is carrying something
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    //Player is carrying something that can be fried
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    _cookingProcessSO = GetCookingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    _currState = State.Cooking;
                    _fryingTimer = 0f;

                    //    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    //    {
                    //        progressNormalized = _fryingTimer / _cookingProcessSO.cookingTimerMax
                    //    });
                    //}
                }
                else
                {
                    //player is not carrying anything

                }
            }
        }
        else //there is a kitchen object here
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //player is carrying a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                        //OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        //{
                        //    progressNormalized = 0f
                        //});
                    }

                }

            }
            else
            {
                //player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
                _currState = State.Idle;

                //OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                //{
                //    progressNormalized = 0f
                //});
            }
        }

        
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CookingProcessSO cookingProcessSO = GetCookingRecipeSOWithInput(inputKitchenObjectSO);
        if (cookingProcessSO != null)
        {
            return cookingProcessSO.output;
        }
        else
        {
            return null;
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CookingProcessSO cookingProcessSO = GetCookingRecipeSOWithInput(inputKitchenObjectSO);
        return cookingProcessSO != null;
    }

    private CookingProcessSO GetCookingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CookingProcessSO cookingProcess in _cookingProcessSOArray)
        {
            if (cookingProcess.input == inputKitchenObjectSO)
            {
                return cookingProcess;
            }
        }
        return null;
    }

    private BurningProcessSO GetBurningProcessSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (BurningProcessSO burningProcessSO in burningProcessSOArray)
        {
            if (burningProcessSO.input == inputKitchenObjectSO)
            {
                return burningProcessSO;
            }
        }
        return null;
    }
}
