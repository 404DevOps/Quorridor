using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class RulesScript : MonoBehaviour
{
    public GameObject mainMenu;
    private VisualElement root;

    private Label ruleText;
    private VisualElement ruleImage;

    //navigation
    private Button exit;
    private Button previous;
    private Button next;
    private int currentPage;

    //data
    private Rulebook ruleBook;

    // Start is called before the first frame update
    void OnEnable()
    {
        //initialize ui components
        root = GetComponent<UIDocument>().rootVisualElement;
        previous = root.Q<Button>("PreviousPage");
        next = root.Q<Button>("NextPage");
        exit = root.Q<Button>("ExitButton");

        ruleImage = root.Q<VisualElement>("ruleImage");
        ruleText = root.Q<Label>("ruleText");

        //hook up events
        previous.clicked += PreviousPage;
        next.clicked += NextPage;
        exit.clicked += Exit;

        //initialize rule book
        currentPage = 0;
        previous.SetEnabled(false);
        var asset = Resources.Load<TextAsset>("rulebook");
        ruleBook = JsonUtility.FromJson<Rulebook>(asset.text);

        LoadPage(currentPage);
    }

    //private void CreateRuleBook()
    //{
    //    var book = new Rulebook();
    //    book.Pages = new List<RulePage>();

    //    book.Pages.Add(new RulePage { pageNumber = 1, imagepath = "asdf.jpg", text = "lorem ipsum", hasImage = false });
    //    book.Pages.Add(new RulePage { pageNumber = 2, imagepath = "asdf.jpg", text = "lorem ipsum", hasImage = false });

    //    var json = JsonUtility.ToJson(book);
    //    Debug.Log(json);
    //}

    void LoadPage(int pageNumber)
    {
        Debug.Log("Page: " + pageNumber);

        var page = ruleBook.Pages.FirstOrDefault(p => p.pageNumber == pageNumber);
        ruleText.text = page.text;
        if (page.hasImage)
        {
            ruleImage.style.display = DisplayStyle.Flex;
            ruleImage.style.backgroundImage = Background.FromTexture2D(Resources.Load<Texture2D>(page.imagePath));
        }
        else 
        {
            ruleImage.style.display = DisplayStyle.None;
            ruleImage.style.backgroundImage = null;
        }
    }

    void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            next.SetEnabled(true);

            if (currentPage == 0)
                previous.SetEnabled(false);

            LoadPage(currentPage);
        }
    }
    void NextPage()
    {
        if (currentPage < ruleBook.Pages.Count)
        {
            currentPage++;
            previous.SetEnabled(true);

            if (currentPage == ruleBook.Pages.Count -1)
                next.SetEnabled(false);

            LoadPage(currentPage);
        }
    }
    void Exit()
    {
        mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }
}

