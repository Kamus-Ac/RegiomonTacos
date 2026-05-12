using UnityEngine;
using System.Collections.Generic;
using System;

public class PlateKitchenObject : KitchenObject
{
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;    
    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectSO kitchenObjectSO;
    }

    private List<KitchenObjectSO> _tacoIngredientsInPlateListSO;
    [SerializeField] private List<KitchenObjectSO> _validKitchenObjectList;

    private void Awake()
    {
        _tacoIngredientsInPlateListSO = new List<KitchenObjectSO>();
    }


    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        if (!_validKitchenObjectList.Contains(kitchenObjectSO))
        {
            //ingredient is not valid for this plate
            return false;
        }

        if (_tacoIngredientsInPlateListSO.Contains(kitchenObjectSO))
            return false;

        else
        {

            _tacoIngredientsInPlateListSO.Add(kitchenObjectSO);

            OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
            {
                kitchenObjectSO = kitchenObjectSO
            });

            return true;
        }

        
    }

    public List<KitchenObjectSO> GetTacoIngredientsInPlateListSO()
    {
        return _tacoIngredientsInPlateListSO;
    }

}
