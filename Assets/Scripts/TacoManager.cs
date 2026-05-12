using System;
using System.Collections.Generic;
using UnityEngine;

public class TacoManager : MonoBehaviour
{

    public class OnRecipeCompletedEventArgs : EventArgs
    {
        public Customer customer;
    }

    public event EventHandler<OnRecipeCompletedEventArgs> OnRecipeCompleted;

    public class OnRecipeFailedEventArgs : EventArgs
    {
        public Customer customer;
    }
    public event EventHandler<OnRecipeFailedEventArgs> OnRecipeFailed;


    public event EventHandler OnOrdersChanged; //UI




    public static TacoManager Instance { get; private set; }


    [SerializeField] private ValidTacoListSO _validTacoList;

    public class CustomerOrder
    {
        public Customer customer;
        public TacoSO tacoSO;
    }

    private List<CustomerOrder> _generatedOrder;
    private int _maxRecipeCount = 4;



    private void Awake()
    {
        Instance = this;

        _generatedOrder = new List<CustomerOrder>();
    }



    public TacoSO GenerateTacoForCustomer(Customer customer)
    {
        if (_generatedOrder.Count >= _maxRecipeCount)
            return null;

        TacoSO generatedTacoSO = _validTacoList.validTacoOrdersList[UnityEngine.Random.Range(0, _validTacoList.validTacoOrdersList.Count)];
        _generatedOrder.Add(new CustomerOrder { customer = customer, tacoSO = generatedTacoSO });

        OnOrdersChanged?.Invoke(this, EventArgs.Empty);

        return generatedTacoSO;
    }




    public void DeliverOrder(PlateKitchenObject plateKitchenObject, Customer customer)
    {
        for (int i = 0; i < _generatedOrder.Count; i++)
        {
            CustomerOrder generatedOrder = _generatedOrder[i];

            // Verify this order belongs to this customer
            if (generatedOrder.customer == customer)
            {

                TacoSO generatedTacoSO = generatedOrder.tacoSO;

                // Verify ingredient count
                if (generatedTacoSO.kitchenObjectSOList.Count ==
                    plateKitchenObject.GetTacoIngredientsInPlateListSO().Count)
                {
                    bool plateContentsMatchesRecipe = true;

                    // Check every recipe ingredient
                    foreach (KitchenObjectSO recipeKitchenObjectSO in generatedTacoSO.kitchenObjectSOList)
                    {
                        bool ingredientFound = false;

                        // Search ingredient in plate
                        foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetTacoIngredientsInPlateListSO())
                        {
                            if (plateKitchenObjectSO == recipeKitchenObjectSO)
                            {
                                ingredientFound = true;
                                break;
                            }
                        }

                        // Missing ingredient
                        if (!ingredientFound)
                        {
                            plateContentsMatchesRecipe = false;
                            break;
                        }
                    }

                    // matches
                    if (plateContentsMatchesRecipe)
                    {
                        _generatedOrder.RemoveAt(i);

                        OnRecipeCompleted?.Invoke(this, new OnRecipeCompletedEventArgs
                        {
                            customer = customer
                        });

                        OnOrdersChanged?.Invoke(this, EventArgs.Empty);

                        return;
                    }
                }

                // Wrong recipe for this customer
                Debug.Log("Incorrect Recipe Delivered!");

                OnRecipeFailed?.Invoke(this, new OnRecipeFailedEventArgs
                {
                    customer = customer
                });

                return;
            }
        }
    }

    public List<CustomerOrder> GetGeneratedTacoOrders()
    {
        return _generatedOrder;
    }

    public void RemoveCustomerOrder(Customer customer)
    {
        for (int i = 0; i < _generatedOrder.Count; i++)
        {
            if (_generatedOrder[i].customer == customer)
            {
                _generatedOrder.RemoveAt(i);

                OnOrdersChanged?.Invoke(this, EventArgs.Empty);

                return;
            }
        }
    }

}
