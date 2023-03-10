using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private CuttingReceipeSO[] cuttingReceipeSOArray;

    public override void Interact(Player player)
    {
        if(!HasKitchenObject())
        {
            //There is no kitchen object here   
            if(player.HasKitchenObject())
            {
                //player has something
                if(HasOutputForInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    //player carrying something that can be cut
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                }
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
                //player has something
            }
            else
            {
                //player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }

        }

    }

    public override void InteractAlternate(Player player)
    {
        if(HasKitchenObject() && HasOutputForInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            KitchenObjectSO output = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
            GetKitchenObject().DestroySelf();

            KitchenObject.SpawnKitchenObject(output, this);    

        }
    }

    private bool HasOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach(CuttingReceipeSO cuttingReceipeSO in cuttingReceipeSOArray)
        {
            if(cuttingReceipeSO.input == inputKitchenObjectSO)
            {
                return true;
            }
        }
        return false;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach(CuttingReceipeSO cuttingReceipeSO in cuttingReceipeSOArray)
        {
            if(cuttingReceipeSO.input == inputKitchenObjectSO)
            {
                return cuttingReceipeSO.output;
            }
        }
        return null;
    }

}
