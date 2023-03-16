using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public event EventHandler OnReceipeSpawned;
    public event EventHandler OnReceipeCompleted;
    public event EventHandler OnReceipeSuccess;
    public event EventHandler OnReceipeFailed;  

    public static DeliveryManager Instance {get; private set;}

    [SerializeField] private ReceipeListSO receipeListSO;

    private List<ReceipeSO> waitingReceipeSOList;
    private float spawnReceipeTimer;
    private float spawnReceipeTimerMax = 4f;
    private int waitingReceipeMax = 4;
    private int successfulReceipesDeliveredAmount;

    private void Awake() 
    {
        Instance = this;
        waitingReceipeSOList = new List<ReceipeSO>();    
    }

    private void Update() 
    {
        spawnReceipeTimer -= Time.deltaTime;

        if(spawnReceipeTimer <= 0f)
        {
            spawnReceipeTimer = spawnReceipeTimerMax;

            if(waitingReceipeSOList.Count < waitingReceipeMax)
            {
                ReceipeSO waitingReceipeSO = receipeListSO.receipeSOList[UnityEngine.Random.Range(0, receipeListSO.receipeSOList.Count)];
                waitingReceipeSOList.Add(waitingReceipeSO);

                OnReceipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }    
    }

    public void DeliverReceipe(PlateKitchenObject plateKitchenObject)
    {
        for(int i = 0; i < waitingReceipeSOList.Count; i++)
        {
            ReceipeSO waitingReceipeSO = waitingReceipeSOList[i];

            if(waitingReceipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                //has the same number of ingredients
                bool plateContentsMatchesReceipe = true;
                foreach(KitchenObjectSO receipeKitchenObjectSO in waitingReceipeSO.kitchenObjectSOList)
                {
                    //cycling through all ingredients in the receipe
                    bool ingredientFound = false;
                    foreach(KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        //cycling throught all ingredients in the plate
                        if(receipeKitchenObjectSO == plateKitchenObjectSO)
                        {
                            //ingredients matches
                            ingredientFound = true;
                            break;
                        }
                    }
                    if(!ingredientFound)
                    {
                        //this receipe ingredient was not found on the plate
                        plateContentsMatchesReceipe = false; 
                    }
                }
                if(plateContentsMatchesReceipe)
                {
                    waitingReceipeSOList.RemoveAt(i);
                    //player delivered the correct receipe
                    OnReceipeCompleted?.Invoke(this, EventArgs.Empty);
                    OnReceipeSuccess?.Invoke(this, EventArgs.Empty);

                    successfulReceipesDeliveredAmount++;

                    return;
                }
            }
        }
        //No matches found
        //player did not deliver a correct receipe
        OnReceipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<ReceipeSO> GetWaitingReceipeSOList()
    {
        return waitingReceipeSOList;
    }

    public int GetSuccessfulReceipesDeliveredAmount()
    {
        return successfulReceipesDeliveredAmount;
    }
}
