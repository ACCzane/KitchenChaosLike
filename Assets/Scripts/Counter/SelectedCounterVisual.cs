using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{

    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGameObjectArray;

    //Later than Awake, for waiting Player instance to be properlly set
    private void Start() {
        Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
    }

    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEvenetArgs e)
    {
        if(e.selectedCounter == baseCounter){
            Show();
        }else{
            Hide();
        }
    }

    private void Show(){
        foreach(GameObject visualGameObject in visualGameObjectArray){
            visualGameObject.SetActive(true);
        }
    }

    private void Hide(){
        foreach(GameObject visualGameObject in visualGameObjectArray){
            visualGameObject.SetActive(false);
        }
    }
}
