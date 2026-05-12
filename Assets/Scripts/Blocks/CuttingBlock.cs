using System;
using UnityEngine;

public class CuttingCounter : BaseBlock//, IHasProgress
{

    //public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    [SerializeField] private CuttingProcessSO[] _cuttingProcessSOArray;
    private int _cuttingProgress;


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
                    //Player is carrying something that can be cut
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    _cuttingProgress = 0;

                    CuttingProcessSO cuttingProcessSO = GetCuttingProcessSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    //OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    //{
                    //    progressNormalized = (float)_cuttingProgress / cuttingProcessSO.cuttingProgressMax
                    //});
                }
            }
            else
            {
                //player is not carrying anything

            }
        }
        else //there is a kitchen object here
        {
            if (player.HasKitchenObject())
            {
                //player is carrying something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //player is carrying a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                    }

                }

            }
            else
            {
                //player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);

                //OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                //{
                //    progressNormalized = 0f
                //});
            }
        }

    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            //there is a kitchen object here and it can be cut
            _cuttingProgress++;
            CuttingProcessSO cuttingProcessSO = GetCuttingProcessSOWithInput(GetKitchenObject().GetKitchenObjectSO());

            //OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            //{
            //    progressNormalized = (float)_cuttingProgress / cuttingProcessSO.cuttingProgressMax
            //});


            if (_cuttingProgress >= cuttingProcessSO.cuttingProgressMax)
            {
                //cutting progress is maxed
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

                GetKitchenObject().DestroySelf();

                KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
            }
        }
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingProcessSO cuttingProcessSO = GetCuttingProcessSOWithInput(inputKitchenObjectSO);
        if (cuttingProcessSO != null)
        {
            return cuttingProcessSO.output;
        }
        else
        {
            return null;
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingProcessSO cuttingProcessSO = GetCuttingProcessSOWithInput(inputKitchenObjectSO);
        return cuttingProcessSO != null;
    }

    private CuttingProcessSO GetCuttingProcessSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingProcessSO cuttingProcessSO in _cuttingProcessSOArray)
        {
            if (cuttingProcessSO.input == inputKitchenObjectSO)
            {
                return cuttingProcessSO;
            }
        }
        return null;
    }
}
