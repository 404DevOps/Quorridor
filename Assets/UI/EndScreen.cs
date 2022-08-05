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

        int player = WinnerName == "Player1" ? 1 : 2;
        playerWonText.text = "Player " + player + " wins the Game!";
        playerWonText.style.color = player == 1 ? Color.yellow : Color.blue;
        backToMenu.clicked += BackToMenu;
        backToMenu.RegisterCallback<MouseOverEvent>(MouseOver);


        //disable HUD
        FindObjectOfType<HUDScript>().gameObject.SetActive(false);

    }

    void MouseOver(MouseOverEvent evt)
    {
        SoundEngine.Instance.PlayHover();
    }

    // Update is called once per frame
    void BackToMenu()
    {
        SoundEngine.Instance.PlayClicked();
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
