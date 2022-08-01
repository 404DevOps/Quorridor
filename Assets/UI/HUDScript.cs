using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HUDScript : MonoBehaviour
{
    VisualElement root;
    Button btnRotateWall;
    Button btnRotateCam;

    Label lblCurrentPlayer;

    public CameraScript mainCamera;

    void OnEnable()
    {
        InitializeHUD();
    }

    void SwitchViewClicked() 
    {
        mainCamera.SwitchView();
    }
    void RotateWallClicked() 
    {
        if (GameManager.Instance.currentWall != null)
            GameManager.Instance.currentWall.Rotate();
    }

    public void SwitchCurrentPlayer(bool isPlayerOne)
    {
        if (lblCurrentPlayer == null)
        {
            InitializeHUD();
        }

        lblCurrentPlayer.text = isPlayerOne ? "Current Player: Yellow" : "Current Player: Blue";
    }

    private void InitializeHUD()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        lblCurrentPlayer = root.Q<Label>("CurrentPlayer");

        btnRotateCam = root.Q<Button>("btnRotateCam");
        btnRotateCam.clicked += SwitchViewClicked;

        btnRotateWall = root.Q<Button>("btnRotateWall");
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            btnRotateWall.style.visibility = Visibility.Hidden;
        }
        else
        {

            btnRotateWall.clicked += RotateWallClicked;
        }
    }
}
