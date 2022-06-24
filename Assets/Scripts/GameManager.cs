using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject selectedObject;
    public static GameManager Instance;
    //game info
    public int currentPlayer;
    public int[] players = { 1, 2 };

    public List<BlockedPath> blockedPaths;
    public PathFinder pathFinder;

    public GameObject endGameMenu;

    //references
    public HUD hud;

    public bool IsGameRunning { get; internal set; }

    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(this.gameObject);
        
        SetRandomPlayer();
        IsGameRunning = true;
        blockedPaths = new List<BlockedPath>();
        pathFinder = new PathFinder();
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

    internal void GameOver(string name)
    {
        
        var screen = endGameMenu.GetComponent<EndScreen>();
        screen.WinnerName = name;
        endGameMenu.SetActive(true);
        IsGameRunning = false;

        //Show Game Over HUD with Rematch Button.
    }
}
