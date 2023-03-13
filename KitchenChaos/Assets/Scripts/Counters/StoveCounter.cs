using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BaseCounter,IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    [SerializeField] private FryingReceipeSO[] fryingReceipeSOArray;
    [SerializeField] private BurningReceipeSO[] burningReceipeSOArray;

    private FryingReceipeSO fryingReceipeSO;
    private BurningReceipeSO burningReceipeSO;

    private State state;
    private float fryingTimer;
    private float burningTimer;

    private void Start() 
    {
        state = State.Idle;    
    }

    private void Update() 
    {
        if(HasKitchenObject())
        {
            switch(state)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;
                    Debug.Log(fryingTimer);

                    if(fryingTimer >= fryingReceipeSO.fryingTimerMax)
                    {
                        //fried -> destroy raw meat object
                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(fryingReceipeSO.output, this);

                        state = State.Fried;
                        burningTimer = 0f;
                        burningReceipeSO = GetBurningReceipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs{
                            state = state
                        });
                    }

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
                        progressNormalized = fryingTimer / fryingReceipeSO.fryingTimerMax
                    });

                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
                        progressNormalized = burningTimer / burningReceipeSO.burningTimerMax
                    });


                    if(burningTimer >= burningReceipeSO.burningTimerMax)
                    {
                        //burned -> destroy cooked meat object
                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(burningReceipeSO.output, this);

                        state = State.Burned;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs{
                            state = state
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
                            progressNormalized = 0f
                        });

                    }
                    break;
                case State.Burned:
                    break;
            }
        }
    }

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
                    //player carrying something that can be fried
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    fryingReceipeSO = GetFryingReceipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    state = State.Frying;
                    fryingTimer = 0f;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs{
                        state = state
                    });

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
                        progressNormalized = 0f
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
                //player has something
            }
            else
            {
                //player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);

                state = State.Idle;

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs{
                    state = state
                });

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
                    progressNormalized = 0f
                });

            }

        }
    }

    private bool HasOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingReceipeSO fryingReceipeSO = GetFryingReceipeSOWithInput(inputKitchenObjectSO);

        if(fryingReceipeSO != null)
            return true;
        else
            return false;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingReceipeSO fryingReceipeSO = GetFryingReceipeSOWithInput(inputKitchenObjectSO);

        if(fryingReceipeSO != null)
            return fryingReceipeSO.output;
        else
            return null;
    }

    private FryingReceipeSO GetFryingReceipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach(FryingReceipeSO fryingReceipeSO in fryingReceipeSOArray)
        {
            if(fryingReceipeSO.input == inputKitchenObjectSO)
            {
                return fryingReceipeSO;
            }
        }
        return null;
    }

    private BurningReceipeSO GetBurningReceipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach(BurningReceipeSO burningReceipeSO in burningReceipeSOArray)
        {
            if(burningReceipeSO.input == inputKitchenObjectSO)
            {
                return burningReceipeSO;
            }
        }
        return null;
    }
}
