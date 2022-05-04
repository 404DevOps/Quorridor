using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    //game info
    public int currentPlayer;
    public int[] players = { 1, 2 };

    //references
    public HUD hud;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        
        SetRandomPlayer();
    }

    public void SetRandomPlayer()
    {
        var index = Random.Range(1, players.Length+1);
        currentPlayer = index;
        hud.UpdateText();
    }

    public void NextPlayer()
    {
        if (currentPlayer == players.Length)
            currentPlayer = 1;
        else
            currentPlayer++;

        hud.UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
