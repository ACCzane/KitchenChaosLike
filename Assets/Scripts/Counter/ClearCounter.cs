using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClearCounter : BaseCounter
{

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player){
        if(!HasKitchenObject()){
            //There is no kitchenObject here
            if(player.HasKitchenObject()){
                //Player is carrying something
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                //Player not carrying anything
            }
        }
        else{
            //There is a kitchenObject here
            if(player.HasKitchenObject()){
                //Player is carrying something
                if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    //Player is holding a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        //Can add ingradient to the plate
                        GetKitchenObject().DestroySelf();
                    }
                    else
                    {
                        //Can not add ingradient to the plate
                    }

                }
                else
                {
                    //Player not holding a plate
                    if(GetKitchenObject().TryGetPlate(out  plateKitchenObject))
                    {
                        //Counter holding a plate
                       if(plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO()))
                       {
                            player.GetKitchenObject().DestroySelf();
                       }
                    }
                }
                
            }
            else
            {
                //Player not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

}
