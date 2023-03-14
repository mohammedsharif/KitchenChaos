using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;

    [SerializeField] private CuttingReceipeSO[] cuttingReceipeSOArray;

    private int cuttingProgress;

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
                    cuttingProgress = 0;

                    CuttingReceipeSO cuttingReceipeSO = GetCuttingReceipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
                        progressNormalized = (float) cuttingProgress / cuttingReceipeSO.cuttingProgressMax
                    });

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
                //player is carrying something
                if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //player has a plate
                    if(plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
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

    public override void InteractAlternate(Player player)
    {
        if(HasKitchenObject() && HasOutputForInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            //there is a kitchen object here and it can be cut
            cuttingProgress++;

            OnCut.Invoke(this, EventArgs.Empty);

            CuttingReceipeSO cuttingReceipeSO = GetCuttingReceipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
                        progressNormalized = (float) cuttingProgress / cuttingReceipeSO.cuttingProgressMax
            });

            if(cuttingProgress >= cuttingReceipeSO.cuttingProgressMax)
            {
                KitchenObjectSO output = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
                GetKitchenObject().DestroySelf();

                KitchenObject.SpawnKitchenObject(output, this);  
            }
  

        }
    }

    private bool HasOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingReceipeSO cuttingReceipeSO = GetCuttingReceipeSOWithInput(inputKitchenObjectSO);

        if(cuttingReceipeSO != null)
            return true;
        else
            return false;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingReceipeSO cuttingReceipeSO = GetCuttingReceipeSOWithInput(inputKitchenObjectSO);

        if(cuttingReceipeSO != null)
            return cuttingReceipeSO.output;
        else
            return null;
    }

    private CuttingReceipeSO GetCuttingReceipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach(CuttingReceipeSO cuttingReceipeSO in cuttingReceipeSOArray)
        {
            if(cuttingReceipeSO.input == inputKitchenObjectSO)
            {
                return cuttingReceipeSO;
            }
        }
        return null;
    }
}
