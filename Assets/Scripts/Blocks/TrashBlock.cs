using UnityEngine;

public class TrashBlock : BaseBlock
    
{
    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            player.GetKitchenObject().DestroySelf();
        }
    }


}
