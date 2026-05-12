using UnityEngine;

public class ContainerBlock : BaseBlock
{

    [SerializeField] private KitchenObjectSO _kitchenObjectSO;


    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            //player is not carrying anything, so give them the kitchen object
            KitchenObject.SpawnKitchenObject(_kitchenObjectSO, player);
        }
    }


}
