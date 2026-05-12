using UnityEngine;

public class BaseBlock : MonoBehaviour, IKitchenObjectParent
{

    [SerializeField] private Transform _counterTopPoint;
    private KitchenObject _kitchenObject;

    public virtual void Interact(Player player)
    {
        Debug.Log("BaseCounter.Interact was called, but it should be implemented in the child class");
    }
    public virtual void InteractAlternate(Player player)
    {

    }

    public Transform GetKitcheObjectFollowTransform()
    {
        return _counterTopPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        _kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return _kitchenObject;
    }

    public void ClearKitchenObject()
    {
        _kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return _kitchenObject != null;
    }





}
