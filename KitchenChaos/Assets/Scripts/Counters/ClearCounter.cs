using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player)
    {
        if(!HasKitchenObject())
        {
            //There is no kitchen object here   
            if(player.HasKitchenObject())
            {
                //player has something
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                //player is not carrying anything
            }
        }
        else
        {
            //There is a kitchen object here
            if(player.HasKitchenObject())
            {
                //player is carrying something
                if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //player has a plate
                    if(plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                    }
                }
                else
                {
                    //player is not carrying a plate but something else
                    if(GetKitchenObject().TryGetPlate(out plateKitchenObject))
                    {
                        //counter has a plate
                        if(plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                        {
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
                }
            }
            else
            {
                //player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }

        }

    }

}
