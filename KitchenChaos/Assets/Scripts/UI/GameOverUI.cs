using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI receipesDeliveredText;

    private void Start() 
    {
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;
        Hide();    
    }

    private void KitchenGameManager_OnStateChanged(object sender, EventArgs e)
    {
        if(KitchenGameManager.Instance.IsGameOver())
        {
            Show();
            receipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfulReceipesDeliveredAmount().ToString();   
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
