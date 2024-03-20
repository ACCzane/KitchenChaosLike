using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class cuttingRecipeSO : ScriptableObject
{
    public KitchenObjectSO inputKitchenObjectSO;
    public KitchenObjectSO outputKitchenObjectSO;
    public int cuttingProgressMax;
}
