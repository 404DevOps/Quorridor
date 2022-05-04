using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Text currentPlayerText;
    public void UpdateText()
    {
        currentPlayerText.text = "Current Player: " + GameManager.Instance.currentPlayer;
        if (GameManager.Instance.currentPlayer == 1)
        {
            currentPlayerText.color = Color.yellow;
        }
        else
        {
            currentPlayerText.color = Color.blue;
        }
    }
}
