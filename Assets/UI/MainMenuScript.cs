using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuScript : MonoBehaviour
{
    private VisualElement root;
    private Button startButton;
    private Button rulesButton;

    public GameObject rulesPanel;
    // Start is called before the first frame update
    void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        startButton = root.Q<Button>("StartButton");
        rulesButton = root.Q<Button>("RulesButton");

        startButton.clicked += StartButtonClicked;
        rulesButton.clicked += RulesButtonClicked;
    }

    void StartButtonClicked() 
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
    void RulesButtonClicked()
    {
        rulesPanel.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
