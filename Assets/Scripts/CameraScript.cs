using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform lookAt;

    private Vector3 defaultCamPos = new Vector3(0f, 0.6f, 0.2f);
    private Vector3 defaultCamRotation;

    private Vector3 topViewPosition = new Vector3(0, 0.6f, 0.00001f);

    private bool isTopView = false;

    public float rotationSpeed;
    public float scrollAmount = 0.05f;
    public bool isPlayerOneTurn;

    private Vector3 dragOrigin;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = defaultCamPos;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(lookAt);

        //reset camera 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchView();
        }
    }

    public void SwitchView()
    {
        if (isTopView)
        {
            transform.position = defaultCamPos;
            transform.LookAt(lookAt);
            isTopView = false;
        }
        else
        {
            transform.position = topViewPosition;
            transform.LookAt(lookAt);
            isTopView = true;
        }
    }

    //private void MoveCamera()
    //{
    //    //drag camera with right click
    //    if (Input.GetMouseButtonDown(1))
    //    {
    //        dragOrigin = Input.mousePosition;
            
    //    }
    //    if(!Input.GetMouseButton(1) || Input.GetMouseButton(0)) return;

    //    Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
    //    Vector3 move = new Vector3(pos.x, pos.y, 0);

    //    //transform.Translate(move, Space.World);

    //    transform.RotateAround(lookAt.position, Vector3.up, rotationSpeed * move.x);


    //}

    public void RotateBoard()
    {
        transform.RotateAround(lookAt.position, Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void ChangePlayerPOV()
    {
        isPlayerOneTurn = !isPlayerOneTurn;
    }




}
