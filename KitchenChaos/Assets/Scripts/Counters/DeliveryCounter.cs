using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public override void Interact(Player player)
    {
        if(player.HasKitchenObject())
        {
            if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                //only accepts Plates

                DeliveryManager.Instance.DeliverReceipe(plateKitchenObject);

                player.GetKitchenObject().DestroySelf();
            }
        }
    }
    
}
