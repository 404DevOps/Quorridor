using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EndScreen : MonoBehaviour
{
    private VisualElement root;
    private Button backToMenu;

    private Label playerWonText;

    public string WinnerName;
    // Start is called before the first frame update
    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        backToMenu = root.Q<Button>("BackToMenu");
        playerWonText = root.Q<Label>("PlayerWon");

        playerWonText.text = WinnerName + " wins the Game!";
        backToMenu.clicked += BackToMenu;
        
    }

    // Update is called once per frame
    void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
