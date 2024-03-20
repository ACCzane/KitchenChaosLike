using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs: EventArgs{
        public state currentState;
    }

    public enum state{
        Idle,
        Frying,
        Fried,
        Burned
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;
    private float fryingTimer;
    private float burningTimer;
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;

    private state currentState;

    private void Start() {
        currentState = state.Idle;
    }

    private void Update() {

        switch (currentState)
        {
            case state.Idle:
                break;
            case state.Frying:
                fryingTimer += Time.deltaTime;

                fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
                    progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
                });

                if(fryingTimer > fryingRecipeSO.fryingTimerMax){
                    //Fried
                    GetKitchenObject().DestroySelf();

                    KitchenObject.SpawnKitchenObject(fryingRecipeSO.outputKitchenObjectSO, this);

                    burningTimer = 0f;
                    currentState = state.Fried;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs{
                        currentState = currentState
                    });


                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
                        progressNormalized = 0f
                    });
                    
                }
                break;
            case state.Fried:
                burningTimer += Time.deltaTime;

                burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
                    progressNormalized = burningTimer / burningRecipeSO.burningTimerMax
                });

                if(burningTimer > burningRecipeSO.burningTimerMax){
                    //Burned
                    GetKitchenObject().DestroySelf();

                    KitchenObject.SpawnKitchenObject(burningRecipeSO.outputKitchenObjectSO, this);

                    currentState = state.Burned;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
                        progressNormalized = 0f
                    });
                }
                break;
            case state.Burned:
                break;
        }

    }

    public override void Interact(Player player)
    {

        if(!HasKitchenObject()){
            //There is no kitchen object here
            if (player.HasKitchenObject()){
                //Player is carrying something
                if(HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())){
                    //Can be placed
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    currentState = state.Frying;
                    fryingTimer = 0f;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs{
                        currentState = currentState
                    });

                }else{
                    //Cannot be placed
                }
            }else{
                //Player isn't carrying anything
            }
        }else{
            //There is a kitchen object here
            if(!player.HasKitchenObject()){
                //Player not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);

                currentState = state.Idle;

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs{
                    currentState = currentState
                });
                
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs{
                    progressNormalized = 0f
                });

            }else{
                //Player is carrying something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //Player is holding a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        //Can add ingradient to the plate
                        GetKitchenObject().DestroySelf();

                        currentState = state.Idle;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            currentState = currentState
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        });
                    }
                    else
                    {
                        //Can not add ingradient to the plate
                    }
                }
                else
                {
                    //Player not holding a plate
                }
            }
        }
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO){
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        if(fryingRecipeSO != null){
            return fryingRecipeSO.outputKitchenObjectSO;
        }
        return null;
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO){
        FryingRecipeSO cuttingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        return cuttingRecipeSO != null;
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO){
        foreach(FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray){
            if(fryingRecipeSO.inputKitchenObjectSO == inputKitchenObjectSO){
                return fryingRecipeSO;
            }
        }
        return null;
    }

    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO){
        foreach(BurningRecipeSO burningRecipeSO in burningRecipeSOArray){
            if(burningRecipeSO.inputKitchenObjectSO == inputKitchenObjectSO){
                return burningRecipeSO;
            }
        }
        return null;
    }
}
